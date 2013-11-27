#include <stdlib.h>
#include <stdio.h>
#include <dlfcn.h>

void* dlwrap_dlopen(const char* file, int flag)
{
    void* ret;
    ret = dlopen(file, flag);
    return ret;
}

char* dlwrap_dlerror()
{
    char* err;
    err = dlerror();
    return err;
}

void* dlwrap_dlsym(void* handle, const char* symbol)
{
    void* ret;
    ret = dlsym(handle, symbol);
    return ret;
}

int dlwrap_dlclose(void* handle)
{
    int ret;
    ret = dlclose(handle);
    return ret;
}
