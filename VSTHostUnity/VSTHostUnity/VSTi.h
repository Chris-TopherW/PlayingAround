#pragma once
#include "VSTBase.h"

class VSTi : public VSTBase
{
public:
	VSTi(const wchar_t* path, VstBasicParams* p_host);
	~VSTi();
	float* processAudio(long numFrames, int numChannels);
	void midiEvent(int status, int mess1, int mess2, long delaySamps);
	int getNumPluginOutputs();
	void initializeIO();

private:
	int pluginNumOutputs = -1;
	float** pluginOutputs = NULL;
	VstEvent eventHolder;
	VstEvents* multEventsHolder;
	void setNumInOut();
};
