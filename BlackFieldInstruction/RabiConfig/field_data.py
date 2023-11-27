# ******************************************************************
#       /\ /|       @file       field_data.py
#       \ V/        @brief      一个C#标准field的信息
#       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
#       /  |                    
#      /  \\        @Modified   2022/4/30
#    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
# ******************************************************************
# !/usr/bin/env python
# -*- coding: utf-8 -*-
from config_def import GROUP_MARK
from str_util import to_lower_camel_case, fix_str


class FieldData:
    # brief
    # private
    def __init__(self, field_name: str, annotate: str, field_type: str):
        self.is_group_field = field_name.startswith(GROUP_MARK)  # 是否按这个字段分组
        self.field_name = to_lower_camel_case(fix_str(field_name))  # 字段名称
        self.annotate = fix_str(annotate)  # 注释
        self.field_type = fix_str(field_type)  # 字段类型
