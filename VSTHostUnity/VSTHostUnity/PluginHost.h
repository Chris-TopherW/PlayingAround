#pragma once

#include "VSTEffect.h"
#include "VSTi.h"

class PluginUtility
{
public:
	~PluginUtility();
	const wchar_t* GetWC(const char *c);
private:
	wchar_t* wc;
};

class VstHost : public VstBasicParams
{
public:
	VstHost();
	void setBlockSize(int p_blocksize);
	int loadEffect(char* path); //returns index
	int loadInstrument(char* path); //returns index
	VSTEffect& getEffect(int index);
	VSTi& getInstrument(int index);
	//void unloadEffect(int index);
	//void unloadInstrument(int index); 
		
private:
	std::vector<VSTEffect> audioEffects;
	std::vector<VSTi> instruments;
	PluginUtility pluginUtility;
};
