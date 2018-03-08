#include "PluginHost.h"

VstHost::VstHost()
{

}

int VstHost::loadEffect(char* path)
{
	//VSTEffect effect(pluginUtility.GetWC(path), this);
	VSTEffect effect(_T("C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll"), this);
	
	int index = audioEffects.size(); //0 indexed
	effect.setVstIndex(index);
	audioEffects.push_back(effect);
	return index; 
}

int VstHost::loadInstrument(char* path)
{
	VSTi instrument(pluginUtility.GetWC(path), this);
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
#ifdef isUnityDLL
		Debug::Log("Error: trying to access non-existent audio effect");
#elif isConsoleVersion
		cout << "Error: trying to access non-existent audio effect" << endl;
#endif
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
#ifdef isUnityDLL
	Debug::Log("Error: trying to access non-existent vst instrument");
#elif isConsoleVersion
		cout << "Error: trying to access non-existent vst instrument"  << endl;
#endif
		return instruments[index];
	}
}
	
const wchar_t* PluginUtility::GetWC(const char *c)
{
	const size_t cSize = strlen(c) + 1;
	if (wc != nullptr)
	{
		delete(wc);
	}
	wc = new wchar_t[cSize];
	mbstowcs(wc, c, cSize);

	return wc;
}

PluginUtility::~PluginUtility()
{
	delete(wc);
}
