using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class AudioTone : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["MUSIC_VOLUME"] = (object)((dynamic)base.Info.Args).Settings["MUSIC_VOLUME"];
		Set("SET_VOLUME_SETTINGS", dictionary);
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2["AUDIO_MODE"] = (object)((dynamic)base.Info.Args).Settings["AUDIO_MODE"];
		dictionary2["SPEAKERPHONE"] = (object)((dynamic)base.Info.Args).Settings["SPEAKERPHONE"];
		Set("SET_AUDIO_PATH_SETTINGS", dictionary2);
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		dictionary3["TONE_LENGTH"] = (object)((dynamic)base.Info.Args).Settings["TONE_LENGTH"];
		dictionary3["LEFT_VOLUME"] = (object)((dynamic)base.Info.Args).Settings["LEFT_VOLUME"];
		dictionary3["RIGHT_VOLUME"] = (object)((dynamic)base.Info.Args).Settings["RIGHT_VOLUME"];
		dictionary3["NUMBER_OF_LOOPS"] = (object)((dynamic)base.Info.Args).Settings["NUMBER_OF_LOOPS"];
		Set("SET_AUDIO_TONE_SETTINGS", dictionary3);
		Tell("TURN_ON_STEREO_TONE");
		string text = ((dynamic)base.Info.Args).PromptText.ToString();
		text = Smart.Locale.Xlate(text);
		Result result = (Result)Prompt(((dynamic)base.Info.Args).PromptType.ToString(), text);
		Tell("TURN_OFF_TONE");
		LogResult(result);
	}
}
