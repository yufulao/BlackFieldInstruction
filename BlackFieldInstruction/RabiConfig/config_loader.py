# ******************************************************************
#       /\ /|       @file       config_loader.py
#       \ V/        @brief      配置加载器
#       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
#       /  |                    
#      /  \\        @Modified   2022/8/17
#    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
# ******************************************************************
# !/usr/bin/env python
# -*- coding: utf-8 -*-
import codecs
import json
import os

CONFIG_PATH = "./config.json"


class ConfigLoader:
    # brief
    # private
    def __init__(self):
        self._config_data = None
        if not os.path.exists(CONFIG_PATH):
            self._config_file = codecs.open(CONFIG_PATH, "a", "utf-8")
            config_data = json.dumps(self.get_default_config(), indent=4)
            self._config_file.write(config_data)
            self._config_file.close()

    # brief 加载配置
    # public
    def load_config(self):
        print("开始加载配置")
        self._config_file = codecs.open(CONFIG_PATH, "r", "utf-8")  # 清单文件 读权限
        config_data_str = self._config_file.read()
        self._config_data = json.loads(config_data_str)

    # brief 获取配置数据
    # public
    def get_config_data(self):
        return self._config_data

    # brief 获取默认配置
    # private
    @staticmethod
    def get_default_config():
        config_json = {
            "MANAGER_NAME": "ConfigManager.cs",
            "MANAGER_TARGET_FOLDER": "../RabiProject/Assets/Scripts/Core/Manager/CommonManager/ConfigManager",
            "XLSX_FOLDER": "./xlsx",
            "DATA_TARGET_FOLDER": "../RabiProject/Assets/AddressableAssets/Config",
            "SCRIPTS_TARGET_FOLDER": "../RabiProject/Assets/Scripts/Config",
            "MANAGER_PATH": "../RabiProject/Assets/Scripts/Core/Manager/CommonManager/ConfigManager/ConfigManager.cs",
            "MANIFEST_PATH": "./xlsx/manifest.txt",
            "SUFFIX": ".cs",
            "ASSET_MANAGER_INIT_CODE": "AssetManager.Instance.OnInit();",
            "ENUM_TARGET_FOLDER": "../RabiProject/Assets/Scripts/ConfigEnum",
            "IS_ATTRIBUTE_EXIST": True,
            "ENUM_MARK": "EnumName",
            "ANNOTATE_MARK": "Annotate",
            "GROUP_MARK": "*",
            "NAME_SPACE_NAME": "Rabi",
        }
        return config_json
