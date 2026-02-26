using System;
using System.IO;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace SmartDevice;

public class FastbootRaw
{
	public enum Status
	{
		Fail,
		Okay,
		Data,
		Info,
		Unknown
	}

	public class Response
	{
		public Status Status { get; set; }

		public string Payload { get; set; }

		public byte[] RawData { get; set; }

		public Response(Status status, string payload)
		{
			Status = status;
			Payload = payload;
		}
	}

	private const int HEADER_SIZE = 4;

	private const int BLOCK_SIZE = 524288;

	private UsbDevice device;

	private string TAG => GetType().FullName;

	public int Timeout { get; set; } = 3000;


	private Status GetStatusFromString(string header)
	{
		return header switch
		{
			"INFO" => Status.Info, 
			"OKAY" => Status.Okay, 
			"DATA" => Status.Data, 
			"FAIL" => Status.Fail, 
			_ => Status.Unknown, 
		};
	}

	public void Connect(UsbDevice targetDevice)
	{
		device = targetDevice;
		if (device == null)
		{
			throw new Exception("No devices available.");
		}
		UsbDevice obj = device;
		IUsbDevice val = (IUsbDevice)(object)((obj is IUsbDevice) ? obj : null);
		if (val != null)
		{
			val.SetConfiguration((byte)1);
			val.ClaimInterface(0);
		}
	}

	public void Disconnect()
	{
		device.Close();
	}

	public Response Command(byte[] command)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		UsbEndpointWriter obj = device.OpenEndpointWriter((WriteEndpointID)1);
		UsbEndpointReader val = device.OpenEndpointReader((ReadEndpointID)129);
		int num = 0;
		obj.Write(command, Timeout, ref num);
		if (num != command.Length)
		{
			throw new Exception($"Failed to write command! Transfered: {num} of {command.Length} bytes");
		}
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = new byte[64];
		string @string;
		Status status;
		do
		{
			int num2 = 0;
			val.Read(array, Timeout, ref num2);
			@string = Encoding.ASCII.GetString(array);
			if (@string.Length < 4)
			{
				status = Status.Unknown;
			}
			else
			{
				string header = new string(@string.Take(4).ToArray());
				status = GetStatusFromString(header);
			}
			stringBuilder.Append(@string.Skip(4).Take(num2 - 4).ToArray());
			stringBuilder.Append("\n");
		}
		while (status == Status.Info);
		string payload = stringBuilder.ToString().Replace("\r", string.Empty).Replace("\0", string.Empty);
		return new Response(status, payload)
		{
			RawData = Encoding.ASCII.GetBytes(@string)
		};
	}

	private void SendDataCommand(long size)
	{
		if (Command($"download:{size:X8}").Status != Status.Data)
		{
			throw new Exception($"Invalid response from device! (data size: {size})");
		}
	}

	private void TransferBlock(FileStream stream, UsbEndpointWriter writeEndpoint, byte[] buffer, int size)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		stream.Read(buffer, 0, size);
		int num = 0;
		writeEndpoint.Write(buffer, Timeout, ref num);
		if (num != size)
		{
			throw new Exception($"Failed to transfer block (sent {num} of {size})");
		}
	}

	public void UploadData(FileStream stream)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		UsbEndpointWriter writeEndpoint = device.OpenEndpointWriter((WriteEndpointID)1);
		UsbEndpointReader val = device.OpenEndpointReader((ReadEndpointID)129);
		long num = stream.Length;
		byte[] buffer = new byte[524288];
		SendDataCommand(num);
		while (num >= 524288)
		{
			TransferBlock(stream, writeEndpoint, buffer, 524288);
			num -= 524288;
		}
		if (num > 0)
		{
			buffer = new byte[num];
			TransferBlock(stream, writeEndpoint, buffer, (int)num);
		}
		byte[] array = new byte[64];
		int num2 = 0;
		val.Read(array, Timeout, ref num2);
		string @string = Encoding.ASCII.GetString(array);
		if (@string.Length < 4)
		{
			throw new Exception($"Invalid response from device: {@string}");
		}
		string header = new string(@string.Take(4).ToArray());
		if (GetStatusFromString(header) != Status.Okay)
		{
			throw new Exception($"Invalid status: {@string}");
		}
	}

	public void UploadData(string path)
	{
		using FileStream stream = new FileStream(path, FileMode.Open);
		UploadData(stream);
	}

	public Response Command(string command)
	{
		return Command(Encoding.ASCII.GetBytes(command));
	}
}
