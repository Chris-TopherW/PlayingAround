#include "VSTEffect.h"

VSTEffect::VSTEffect(std::string& path, int p_samplerate, int p_blocksize) : VSTBase(path)
{
	samplerate = p_samplerate;
	blocksize = p_blocksize;
	if (getPluginReady())
	{
		initializeIO();
		startPlugin();
	}
}

VSTEffect::~VSTEffect()
{
	plugin->dispatcher(plugin, effMainsChanged, 0, 0, NULL, 0.0f);

	if (pluginInputsStartPtr != nullptr)
	{
		for (int i = 0; i < pluginInputs.size(); i++)
		{
			free(pluginInputsStartPtr[i]);
		}
		free(pluginInputsStartPtr);
	}
	
	if (pluginOutputsStartPtr != nullptr)
	{
		for (int i = 0; i < pluginOutputs.size(); i++)
		{
			free(pluginOutputsStartPtr[i]);
		}
		free(pluginOutputsStartPtr);
	}
}

float* VSTEffect::processAudio(float* audioThrough, long numFrames, int numChannels)
{
	silenceChannel(pluginInputs);
	silenceChannel(pluginOutputs);
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

	plugin->processReplacing(plugin, pluginInputsStartPtr, pluginOutputsStartPtr, numFrames);

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
	
	std::vector<float> tempVect;
	tempVect.reserve(blocksize);
	for (int i = 0; i < blocksize; i++)
	{
		tempVect.push_back(0.0f);
	}

	for (int i = 0; i < plugin->numInputs; i++)
	{
		pluginInputs.push_back(tempVect);
	}
	for (int i = 0; i < plugin->numOutputs; i++)
	{
		pluginOutputs.push_back(tempVect);
	}
	
	//hold locations of starts of i/o vectors as raw float** in order to access for plugin.processReplacing
	std::vector<float*> target(pluginInputs.size());
	for (int i = 0; i < pluginInputs.size(); i++)
	{
		target[i] = &*pluginInputs[i].begin();
	}
	pluginInputsStartPtr = (float**)malloc((sizeof(float*) * 2));
	pluginInputsStartPtr[0] = target[0];
	pluginInputsStartPtr[1] = target[1];

	std::vector<float*> target2(pluginOutputs.size());
	for (int i = 0; i < pluginOutputs.size(); i++)
	{
		target2[i] = &*pluginOutputs[i].begin();
	}
	pluginOutputsStartPtr = (float**)malloc((sizeof(float*) * 2));
	pluginOutputsStartPtr[0] = target2[0];
	pluginOutputsStartPtr[1] = target2[1];
}

int VSTEffect::getNumPluginInputs()
{
	if (pluginNumInputs == -1)
	{
		Debug::Log("Error, plugin inputs not initialised");
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
		Debug::Log("Error, plugin outputs not initialised");
		return pluginNumOutputs;
	}
	else
	{
		return pluginNumOutputs;
	}
}
