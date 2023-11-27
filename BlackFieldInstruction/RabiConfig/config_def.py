# ******************************************************************
#       /\ /|       @file       config_def
#       \ V/        @brief      默认配置
#       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
#       /  |
#      /  \\        @Modified   2022/4/24
#    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
# ******************************************************************
# !/usr/bin/env python
# -*- coding: utf-8 -*-

# 配置加载
from config_loader import ConfigLoader

config_loader = ConfigLoader()
config_loader.load_config()
config_data = config_loader.get_config_data()

MANAGER_NAME = config_data["MANAGER_NAME"]  # 管理器名称
MANAGER_TARGET_FOLDER = config_data["MANAGER_TARGET_FOLDER"]  # 管理器存放目录
XLSX_FOLDER = config_data["XLSX_FOLDER"]  # Excel存放目录
DATA_TARGET_FOLDER = config_data["DATA_TARGET_FOLDER"]  # Excel数据存放目录
SCRIPTS_TARGET_FOLDER = config_data["SCRIPTS_TARGET_FOLDER"]  # 生成的代码存放目录
MANAGER_PATH = config_data["MANAGER_PATH"]  # 管理器生成目标路径
MANIFEST_PATH = config_data["MANIFEST_PATH"]  # 管理器生成目标路径
SUFFIX = config_data["SUFFIX"]  # 解析脚本后缀
ASSET_MANAGER_INIT_CODE = config_data["ASSET_MANAGER_INIT_CODE"]  # 资源管理器初始化代码
DEF_TARGET_FOLDER = config_data["DEF_TARGET_FOLDER"]  # 生成的定义存放目录
DEF_MARK = config_data["DEF_MARK"]  # 定义生成特殊标识
ANNOTATE_MARK = config_data["ANNOTATE_MARK"]  # 注释标识
GROUP_MARK = config_data["GROUP_MARK"]  # 组标识 有这个符号的字段值会作为内部组字典的key
NAME_SPACE_NAME = config_data["NAME_SPACE_NAME"]  # 命名空间名称
