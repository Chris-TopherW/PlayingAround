#include "VSTEffect.h"

VSTEffect::VSTEffect(const wchar_t* path, VstBasicParams* p_hostParams) : VSTBase(path)
{
	hostParams = p_hostParams;
	initializeIO();
	startPlugin();
}

VSTEffect::~VSTEffect()
{
	plugin->dispatcher(plugin, effMainsChanged, 0, 0, NULL, 0.0f);
	for (int i = 0; i < getNumPluginInputs(); i++)
	{
		free(pluginInputs[i]);
	}
	free(pluginInputs);

	for (int i = 0; i < getNumPluginOutputs(); i++)
	{
		free(pluginOutputs[i]);
	}
	free(pluginOutputs);
}

float* VSTEffect::processAudio(float* audioThrough, long numFrames, int numChannels)
{
	silenceChannel(pluginInputs, pluginNumInputs, numFrames);
	silenceChannel(pluginOutputs, pluginNumOutputs, numFrames);

	/////////de-interleaver////////
	int j = 0;
	for (int samp = 0; samp < numFrames * numChannels; samp += numChannels)
	{
		pluginInputs[0][j] = audioThrough[samp];
		if (numChannels == 2)
		{
			if (pluginNumInputs == 2)
			{
				//add right chan for stereo in and out
				pluginInputs[1][j] = audioThrough[samp + 1];
			}
			else if (pluginNumInputs == 1)
			{
				//sum left and right ins at half gain
				pluginInputs[0][j] = audioThrough[samp] * 0.5f + audioThrough[samp + 1] * 0.5f;
			}
		}
		else if (numChannels == 1 && pluginNumInputs == 2)
		{
			//send mono input to each plugin channel
			pluginInputs[1][j] = audioThrough[samp];
		}
		j++;
	}

	plugin->processReplacing(plugin, pluginInputs, pluginOutputs, numFrames);

	/////////re-interleaver////////
	j = 0;
	for (int samp = 0; samp < numFrames * numChannels; samp += numChannels)
	{
		audioThrough[samp] = pluginOutputs[0][j];
		if (numChannels == 2)
		{
			if (pluginNumOutputs == 2)
			{
				audioThrough[samp + 1] = pluginOutputs[1][j];
			}
			else if (pluginNumOutputs == 1)
			{
				//add plugin left output to right channel
				audioThrough[samp + 1] = pluginOutputs[0][j];
			}
		}
		else if (numChannels == 1 && pluginNumOutputs == 2)
		{
			//if mono out but plugin is stereo output, sum then half gain
			audioThrough[samp] = pluginOutputs[0][j] * 0.5f + pluginOutputs[1][j] * 0.5f;
		}
		j++;
	}
	return audioThrough;
}

void VSTEffect::initializeIO() {

	pluginInputs = (float**)malloc(sizeof(float*) * plugin->numInputs);
	pluginOutputs = (float**)malloc(sizeof(float*) * plugin->numOutputs);
	for (int channel = 0; channel < plugin->numInputs; channel++) {
		pluginInputs[channel] = (float*)malloc(sizeof(float) * hostParams->blocksize);
	}
	for (int channel = 0; channel < plugin->numOutputs; channel++) {
		pluginOutputs[channel] = (float*)malloc(sizeof(float) * hostParams->blocksize);
	}
}

int VSTEffect::getNumPluginInputs()
{
	if (pluginNumInputs == -1)
	{
#ifdef isUnityDLL
		Debug::Log("Error, plugin inputs not initialised");
#elif isConsoleVersion
		cout << "Error, plugin inputs not initialised" << endl;
#endif
		return pluginNumInputs;
	}
	else
	{
		return pluginNumInputs;
	}
}

int VSTEffect::getNumPluginOutputs()
{
	if (pluginNumOutputs == -1)
	{
#ifdef isUnityDLL
		Debug::Log("Error, plugin outputs not initialised");
#elif isConsoleVersion
		cout << "Error, plugin outputs not initialised" << endl;
#endif
		return pluginNumOutputs;
	}
	else
	{
		return pluginNumOutputs;
	}
}
