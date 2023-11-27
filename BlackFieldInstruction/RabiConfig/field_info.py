# ******************************************************************
#       /\ /|       @file       field_info.py
#       \ V/        @brief      复杂field的信息 可以是自定义结构、自定义结构的数组、基础类型数组
#       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
#       /  |                    
#      /  \\        @Modified   2022/4/30
#    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
# ******************************************************************
# !/usr/bin/env python
# -*- coding: utf-8 -*-
import field_data
from str_util import to_lower_camel_case, fix_str


class FieldInfo:
    # brief
    # private
    def __init__(self, parent_class_name: str):
        self.parent_class_name = fix_str(parent_class_name)  # 当前信息所属的class名称
        self.field_data_set = set()  # 当前数据已记录的field data
        self.field_data_list = []  # 当前数据结构信息中的全部数据
        self.appear_count = 0  # 当前数据结构中 任意field出现的次数 用来算数组长度
        self.array_length = 0  # 数组长度 AppearCount/fieldMap的长度 就是当前数据结构的数组长度

    # brief 尝试向当前结构加入新字段
    # public
    def try_add_field(self, data: field_data.FieldData):
        # 当前没有被记录的field 加入map
        if data.field_name not in self.field_data_set:
            self.field_data_set.add(data.field_name)
            self.field_data_list.append(data)
        # 当前结构中 field出现次数++
        self.appear_count += 1
        # 当前数据结构的数组长度
        self.array_length = self.appear_count // len(self.field_data_set)

    # brief 获取当前field的Type
    # public
    def get_field_type(self):
        # 非自定义类型field
        if not self.parent_class_name:
            # 非自定义结构的field只有一个field data
            if not self.field_data_list or len(self.field_data_list) != 1:
                print(f"当前field info解析异常")
                return
            # 数组的情况
            data = self.field_data_list[0]
            if self.array_length > 1:
                return f"{data.field_type}[]"
            return data.field_type
        # 自定义类型field
        if self.array_length > 1:
            return f"{self.parent_class_name}[]"
        return self.parent_class_name

    # brief 获取当前field的名称
    # public
    def get_field_name(self):
        # 非自定义类型field
        if not self.parent_class_name:
            if not self.field_data_list or len(self.field_data_list) != 1:
                print(f"当前field info解析异常")
                return
            data = self.field_data_list[0]
            field_name = data.field_name
        else:
            # 自定义类型field 数组
            field_name = to_lower_camel_case(self.parent_class_name)
        return self.array_length > 1 and f"{field_name}s" or field_name

    # brief 获取当前field的注释
    # public
    def get_field_annotate(self):
        # 非自定义类型field
        if not self.parent_class_name:
            if not self.field_data_list or len(self.field_data_list) != 1:
                print(f"当前field info解析异常")
                return
            data = self.field_data_list[0]
            return data.annotate
        # 自定义类型field
        return self.parent_class_name

    # brief 是否是组标记字段
    # public
    def is_group_field(self) -> bool:
        # 自定义类型字段 不能作为组标记
        if self.parent_class_name:
            return False
        if not self.field_data_list or len(self.field_data_list) != 1:
            print(f"当前field info解析异常")
            return False
        data = self.field_data_list[0]
        return data.is_group_field
