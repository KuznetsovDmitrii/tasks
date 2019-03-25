//
//  ID.cpp
//  task3
//
//  Created by Дмитрий Кузнецов on 13.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#include "Id.h"

ID::ID(){
    name = new char[128];
}

ID::ID(const ID& id){
    strcpy(name, id.name);
    value=id.value;
}

ID& ID::operator=(const ID& id){
    strcpy(name, id.name);
    value=id.value;
    return *this;
}

ID::~ID(){
    //delete[] name;
}

void ID::PutName(char *id_name){
    delete[] name;
    name = new char [strlen(id_name)+1];
    strcpy(name,id_name);
}

void ID::PutValue(int id_value){
    value = id_value;
}

char *ID::GetName(){
    return name;
}

int ID::GetValue(){
    return value;
}