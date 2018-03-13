#include "PluginHost.h"

VstHost::VstHost()
{

}

int VstHost::loadEffect(std::string& path)
{
	//VSTEffect effect(pluginUtility.GetWC(path), this);
	VSTEffect effect(path, *this);
	//"C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll"
	
	int index = audioEffects.size(); //0 indexed
	effect.setVstIndex(index);
	audioEffects.push_back(effect);
	return index; 
}

int VstHost::loadInstrument(std::string& path)
{
	VSTi instrument(path, *this);
	int index = audioEffects.size();
	instrument.setVstIndex(index);
	instruments.push_back(instrument);
	return index;
}

void VstHost::setBlockSize(int p_blocksize)
{
	blocksize = p_blocksize;
}

VSTEffect& VstHost::getEffect(int index)
{
	if (audioEffects.size() >= index)
	{
		return audioEffects[index];
	}
	else
	{
		Debug::Log("Error: trying to access non-existent audio effect");
		return audioEffects[index];
	}
}

VSTi& VstHost::getInstrument(int index)
{
	if (instruments.size() >= index)
	{
		return instruments[index];
	}
	else
	{
	Debug::Log("Error: trying to access non-existent vst instrument");
		return instruments[index];
	}
}
