//
//  ID.h
//  task3
//
//  Created by Дмитрий Кузнецов on 13.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#ifndef __task3__ID__
#define __task3__ID__

#include <string.h>

enum TypeOfID{
    pascalDef = -1,
    pascalInt = 0,
    pascalStr = 1,
};

union set{
    char str[256];
    int integer;
};

class ID{
private:
    char *name;
    int value;
    
public:
    ID();
    ID(const ID& id);
    ID& operator=(const ID& id);
    ~ID();
    
    void PutName(char *name);
    void PutValue(int value);
    
    char *GetName();
    int GetValue();
};

#endif /* defined(__task3__ID__) */