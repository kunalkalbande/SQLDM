//**********************************************************************
//*
//* File: stdafx.h
//*
//* Copyright Idera (Idera, Inc.) 2005
//*
//**********************************************************************
#pragma once


#define WIN32_LEAN_AND_MEAN		// Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>

// TODO: reference additional headers your program requires here
#include <wincrypt.h>
#include <tchar.h>
#include <stdio.h>
#include <memory.h>
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <aclapi.h>

#ifndef DIM
#define DIM(array) (sizeof (array) / sizeof (array[0]))
#endif
