# ******************************************************************
#       /\ /|       @file       manager_generator.py
#       \ V/        @brief      代码管理器生成器
#       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
#       /  |
#      /  \\        @Modified   2022/4/23
#    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
# ******************************************************************
# !/usr/bin/env python
# -*- coding: utf-8 -*-
import codecs
import os

import config_def
from config_def import SUFFIX, MANAGER_PATH, SCRIPTS_TARGET_FOLDER, ASSET_MANAGER_INIT_CODE
from str_util import to_lower_camel_case, get_filename_from_path, get_filename


class ManagerGenerator(object):
    # brief
    # private
    def __init__(self):
        self._manager_file = codecs.open(MANAGER_PATH, "w", "utf-8")

    # brief
    # private
    def __del__(self):
        self._manager_file.close()
        return

    # brief 生成管理器代码
    # public
    def generate_manager(self):
        print("开始生成管理器")
        # 标题
        self.write_header()
        # 代码
        self.write_code()

    # brief 写入标题字符
    # private
    def write_header(self):
        str_list = [
            "// ******************************************************************",
            f"//       /\\ /|       @file       {get_filename_from_path(MANAGER_PATH)}.cs",
            "//       \\ V/        @brief      配置表管理器(由python自动生成)",
            "//       | \"\")       @author     Shadowrabbit, yue.wang04@mihoyo.com",
            "//       /  |",
            "//      /  \\\\        @Modified   2022-04-23 22:40:15",
            "//    *(__\\_\\        @Copyright  Copyright (c)  2022, Shadowrabbit",
            "// ******************************************************************"
        ]
        result = os.linesep.join(str_list)
        self._manager_file.write(result)

    # brief 生成代码
    # private
    # field script_folder 脚本目录
    def write_code(self):
        self._manager_file.write(os.linesep)
        self._manager_file.write(os.linesep)
        self._manager_file.write(f"namespace {config_def.NAME_SPACE_NAME}")
        self._manager_file.write(os.linesep)
        self._manager_file.write("{")
        self.write_class(get_filename_from_path(MANAGER_PATH))
        self._manager_file.write("}")

    # brief 生成类
    # private
    # field script_folder 脚本目录
    def write_class(self, file_name):
        self._manager_file.write(os.linesep)
        self._manager_file.write(f"    public sealed class {file_name} : BaseSingleTon<{file_name}>")
        self._manager_file.write(os.linesep)
        self._manager_file.write("    {")
        self.write_fields()
        self.write_constructor()
        self.write_init_function()
        self._manager_file.write(os.linesep)
        self._manager_file.write("    }")

    # brief 生成字段
    # private
    def write_fields(self):
        if not isinstance(SCRIPTS_TARGET_FOLDER, str):
            return
        self._manager_file.write(os.linesep)
        str_list = []  # 每行字段定义
        for dir_path, dir_names, file_names in os.walk(SCRIPTS_TARGET_FOLDER):
            # 不计算子目录 只计算当前目录下后缀为.cs的脚本
            script_names = list(filter(self.file_filter, file_names))
            for script_name in script_names:
                # 去后缀
                script_name = get_filename(script_name)
                str_list.append(f"        public readonly {script_name} {to_lower_camel_case(script_name)} = "
                                f"new {script_name}();")
        result = os.linesep.join(str_list)
        self._manager_file.write(result)

    # brief 写入构造器
    # private
    def write_constructor(self):
        self._manager_file.write(os.linesep)
        self._manager_file.write(os.linesep)
        self._manager_file.write("        public ConfigManager()")
        self._manager_file.write(os.linesep)
        self._manager_file.write("        {")
        self._manager_file.write(os.linesep)
        self._manager_file.write("            //初始场景有Text的情况 查找翻译文本需要加载资源 因为同为Awake回调 加载顺序可能优于AssetManager 故补充加载")
        self._manager_file.write(os.linesep)
        self._manager_file.write(f"            {ASSET_MANAGER_INIT_CODE}")
        self._manager_file.write(os.linesep)
        self._manager_file.write("            Init();")
        self._manager_file.write(os.linesep)
        self._manager_file.write("        }")

    # brief 生成函数
    # private
    def write_init_function(self):
        self._manager_file.write(os.linesep)
        self._manager_file.write(os.linesep)
        self._manager_file.write("        /// <summary>")
        self._manager_file.write(os.linesep)
        self._manager_file.write("        /// 初始化")
        self._manager_file.write(os.linesep)
        self._manager_file.write("        /// </summary>")
        self._manager_file.write(os.linesep)
        self._manager_file.write("        private void Init()")
        self._manager_file.write(os.linesep)
        self._manager_file.write("        {")
        for dir_path, dir_names, file_names in os.walk(SCRIPTS_TARGET_FOLDER):
            # 不计算子目录 只计算当前目录下后缀为Cfg.cs的脚本
            script_names = list(filter(self.file_filter, file_names))
            for script_name in script_names:
                script_name = get_filename(script_name)
                self._manager_file.write(os.linesep)
                self._manager_file.write(f"            {to_lower_camel_case(script_name)}.Load();")
        self._manager_file.write(os.linesep)
        self._manager_file.write("        }")

    # brief 文件过滤是否通过
    # private
    @staticmethod
    def file_filter(file_name):
        return file_name.endswith(SUFFIX)
