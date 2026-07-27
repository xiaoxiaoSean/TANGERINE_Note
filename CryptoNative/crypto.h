#pragma once

#ifdef CRYPTONATIVE_EXPORTS
#define API __declspec(dllexport)
#else
#define API __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C" {
#endif

	API int Add(int a, int b);

#ifdef __cplusplus
}
#endif