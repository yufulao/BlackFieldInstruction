# ******************************************************************
#       /\ /|       @file       str_util.py
#       \ V/        @brief      字符串工具
#       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
#       /  |                    
#      /  \\        @Modified   2022/4/23
#    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
# ******************************************************************
# !/usr/bin/env python
# -*- coding: utf-8 -*-


# brief 根据文件路径获取文件名
# public
from config_def import GROUP_MARK


def get_filename_from_path(filepath):
    path_split = filepath.split("/")
    file_name = path_split[len(path_split) - 1]
    return get_filename(file_name)


# brief 获取无扩展名的文件名称
# public
def get_filename(file_name):
    name_split = file_name.split(".")
    return name_split[0]


# brief 转换小驼峰
# private
def to_lower_camel_case(field_name):
    if not field_name:
        return field_name
    first_character = field_name[0].lower()
    if len(field_name) <= 1:
        return first_character
    return f"{first_character}{field_name[1:]}"


# brief 转换大驼峰
# private
def to_big_camel_case(field_name):
    if not field_name:
        return field_name
    first_character = field_name[0].upper()
    if len(field_name) <= 1:
        return first_character
    return f"{first_character}{field_name[1:]}"


# brief 修正无效字符
# private
def fix_str(value: str):
    return value.replace("\n", "").replace("\r", "").replace(" ", "").replace(GROUP_MARK, "")
