#pragma once
#include "PluginHost.h"

class ConsoleHost
{
public:
	std::shared_ptr<VstHost> vstHost; //heap allocation as this could get big
	bool hostInitialised = false;
	void initHost();
	void setSampleRate(long p_sampleRate);
	void setHostBlockSize(int p_blocksize);
	int loadEffect(std::string& path);
	int loadInstrument(std::string& path);
	//these need to be raw pointers for C# marshalling
	float* processFxAudio(int vstIndex, float* audioThrough, long numFrames, int numChannels);
	float* processInstAudio(int vstIndex, long numFrames, int numChannels);
};
