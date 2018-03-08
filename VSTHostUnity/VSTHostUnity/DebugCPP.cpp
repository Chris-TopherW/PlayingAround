#include "DebugCPP.h"
#include <sstream>
#include <iostream>
using namespace std;
//#define isConsoleVersion 1
#define isUnityDLL 1

//-------------------------------------------------------------------
void  Debug::Log(const char* message, Color color) {
	
#ifdef isConsoleVersion
	cout << message << endl;
#elif isUnityDLL
	if (callbackInstance != nullptr)
		callbackInstance(message, (int)color, (int)strlen(message));
#endif
}

void  Debug::Log(const std::string message, Color color) {
	
#ifdef isConsoleVersion
	cout << message << endl;
#elif isUnityDLL
	const char* tmsg = message.c_str();
	if (callbackInstance != nullptr)
		callbackInstance(tmsg, (int)color, (int)strlen(tmsg));
#endif
}

void  Debug::Log(const int message, Color color) {
	
#ifdef isConsoleVersion
	cout << message << endl;
#elif isUnityDLL
	std::stringstream ss;
	ss << message;
	send_log(ss, color);
#endif
}

void  Debug::Log(const char message, Color color) {
	
#ifdef isConsoleVersion
	cout << message << endl;
#elif isUnityDLL
	std::stringstream ss;
	ss << message;
	send_log(ss, color);
#endif
}

void  Debug::Log(const float message, Color color) {
	
#ifdef isConsoleVersion
	cout << message << endl;
#elif isUnityDLL
	std::stringstream ss;
	ss << message;
	send_log(ss, color);
#endif
}

void  Debug::Log(const double message, Color color) {
	
#ifdef isConsoleVersion
	cout << message << endl;
#elif isUnityDLL
	std::stringstream ss;
	ss << message;
	send_log(ss, color);
#endif
}

void Debug::Log(const bool message, Color color) {
	
#ifdef isConsoleVersion
	cout << message << endl;
#elif isUnityDLL
	std::stringstream ss;
	if (message)
		ss << "true";
	else
		ss << "false";

	send_log(ss, color);
#endif
}

void Debug::send_log(const std::stringstream &ss, const Color &color) {
	const std::string tmp = ss.str();
	const char* tmsg = tmp.c_str();
	if (callbackInstance != nullptr)
		callbackInstance(tmsg, (int)color, (int)strlen(tmsg));
}
//-------------------------------------------------------------------

//Create a callback delegate
void RegisterDebugCallback(FuncCallBack cb) {
	callbackInstance = cb;
}
