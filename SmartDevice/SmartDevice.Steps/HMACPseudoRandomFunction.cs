using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;

namespace SmartDevice.Steps;

public sealed class HMACPseudoRandomFunction<T> : HMACPseudoRandomFunction where T : HMAC
{
	public HMACPseudoRandomFunction(byte[] input)
		: base(typeof(T), input)
	{
	}
}
public abstract class HMACPseudoRandomFunction : IPseudoRandomFunction, IDisposable
{
	private static readonly Dictionary<Type, Func<byte[], HMAC>> constructors = new Dictionary<Type, Func<byte[], HMAC>>();

	private HMAC hmac;

	private bool disposed;

	public int HashSize => hmac.HashSize / 8;

	protected HMACPseudoRandomFunction(Type hmacType, byte[] input)
	{
		Func<byte[], HMAC> constructor = GetConstructor(hmacType);
		hmac = constructor(input);
	}

	protected Func<byte[], HMAC> GetConstructor(Type type)
	{
		if (!constructors.TryGetValue(type, out var value))
		{
			ConstructorInfo? constructor = type.GetConstructor(new Type[1] { typeof(byte[]) });
			ParameterExpression parameterExpression = Expression.Parameter(typeof(byte[]), "key");
			value = Expression.Lambda<Func<byte[], HMAC>>(Expression.New(constructor, parameterExpression), new ParameterExpression[1] { parameterExpression }).Compile();
			constructors.Add(type, value);
		}
		return value;
	}

	public byte[] Transform(byte[] input)
	{
		return hmac.ComputeHash(input);
	}

	public void Dispose()
	{
		if (!disposed)
		{
			hmac.Clear();
			hmac = null;
			disposed = true;
		}
	}
}
