# ******************************************************************
#       /\ /|       @file       run.py
#       \ V/        @brief      启动器
#       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
#       /  |
#      /  \\        @Modified   2022/4/23
#    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
# ******************************************************************
# !/usr/bin/env python
# -*- coding: utf-8 -*-
import os.path
import shutil
import stat

import data_generator
import def_generator
import manager_generator
import script_generator

# 各个目录
from config_def import MANAGER_TARGET_FOLDER, DATA_TARGET_FOLDER, SCRIPTS_TARGET_FOLDER, MANAGER_NAME, MANAGER_PATH, \
    MANIFEST_PATH, DEF_TARGET_FOLDER

folder_map = {
    "MANAGER_TARGET_FOLDER": MANAGER_TARGET_FOLDER,
    "DATA_TARGET_FOLDER": DATA_TARGET_FOLDER,
    "SCRIPTS_TARGET_FOLDER": SCRIPTS_TARGET_FOLDER,
    "ENUM_TARGET_FOLDER": DEF_TARGET_FOLDER,
}


# brief 运行
# public
def run():
    # 目录检测
    check_folders()
    # 清理残留文件
    clear_cache_files()
    # 生成数据
    generate_data()
    # 生成代码
    generate_code()
    # 生成管理器
    generate_manager()
    # 生成枚举
    generate_enum()
    os.system("pause")


# brief 拷贝目录下全部文件
# private
# from_folder string 起始目录
# to_folder string 目标目录
def copy_folder(from_folder, to_folder):
    if not os.path.exists(from_folder):
        print(f"需要拷贝的目录不存在:{from_folder}")
        return
    if not os.path.exists(to_folder):
        print(f"目标拷贝目录不存在:{to_folder}")
        return
    shutil.copy(from_folder, to_folder)


# brief 目录检测 不存在的目录创建一下
# private
def check_folders():
    print(f"管理器名称:{MANAGER_NAME}")
    print(f"清单文件路径:{MANIFEST_PATH}")
    print(f"管理器存放目录:{MANAGER_TARGET_FOLDER}")
    print(f"Excel数据存放目录:{DATA_TARGET_FOLDER}")
    print(f"生成的代码存放目录:{SCRIPTS_TARGET_FOLDER}")
    for folder in folder_map.values():
        if not os.path.exists(folder):
            os.makedirs(folder)


# brief 清理上次的缓存文件
# private
def clear_cache_files():
    for folder in folder_map.values():
        # 管理器只删自己就好了 别删整个目录
        if folder == MANAGER_TARGET_FOLDER and os.path.exists(MANAGER_PATH):
            remove_file(MANAGER_PATH)
            continue
        # 当前目录下所有文件
        filenames = os.listdir(folder)
        for filename in filenames:
            remove_file(f"{folder}/{filename}")


# brief 移除文件
# private
def remove_file(file_path):
    # 全部权限
    os.chmod(file_path, stat.S_IRWXU)
    os.remove(file_path)


# brief 生成数据
# private
def generate_data():
    generator = data_generator.DataGenerator()
    generator.generate_data()


# brief 生成代码
# private
def generate_code():
    generator = script_generator.ScriptGenerator()
    generator.generate_script()


# brief 生成枚举
# private
def generate_enum():
    generator = def_generator.DefGenerator()
    generator.generate_def()


# brief 生成管理器
# private
def generate_manager():
    generator = manager_generator.ManagerGenerator()
    generator.generate_manager()


if __name__ == '__main__':
    run()
    pass
