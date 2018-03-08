#pragma once
#include "PluginHost.h"
#include <memory>

std::shared_ptr<VstHost> vstHost;
bool hostInitialised = false;
void initHost();
void setSampleRate(long p_sampleRate);
void setHostBlockSize(int p_blocksize);
int loadEffect(char* path);
int loadInstrument(char* path);
float* processFxAudio(int vstIndex, float* audioThrough, long numFrames, int numChannels);
float* processInstAudio(int vstIndex, long numFrames, int numChannels);

