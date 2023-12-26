// ******************************************************************
//       /\ /|       @file       CfgStage.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//Stage.xlsx
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |
//      /  \\        @Modified   2022-04-25 13:25:11
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Rabi
{
    public class RowCfgStage
    {
        public string key; //key
        public int width; //横向格子数
        public int height; //纵向格子数
        public float cellSize; //每个网格的宽高
        public string scenePath; //场景资源路径
        public Dictionary<int,int> commandDic; //关卡可用指令
        public Dictionary<int,int> commandTime; //每个指令时长
        public int stageTime; //关卡的时间上限
        public string nextStageName; //下一关的key
    }

    public class CfgStage
    {
        private readonly Dictionary<string, RowCfgStage> _configs = new Dictionary<string, RowCfgStage>(); //cfgId映射row
        public RowCfgStage this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgStage this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgStage> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgStage Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgStage Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgStage.txt", 3);
            var rows = reader.GetRowCount();
            for (var i = 0; i < rows; ++i)
            {
                var row = reader.GetColValueArray(i);
                var data = ParseRow(row);
                if (!_configs.ContainsKey(data.key))
                {
                    _configs.Add(data.key, data);
                }
            }
        }

        /// <summary>
        /// 解析行
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private RowCfgStage ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 9)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgStage();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.width = CsvUtility.ToInt(rowHelper.ReadNextCol()); //横向格子数
            data.height = CsvUtility.ToInt(rowHelper.ReadNextCol()); //纵向格子数
            data.cellSize = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //每个网格的宽高
            data.scenePath = CsvUtility.ToString(rowHelper.ReadNextCol()); //场景资源路径
            data.commandDic = CsvUtility.ToDictionary<int,int>(rowHelper.ReadNextCol()); //关卡可用指令
            data.commandTime = CsvUtility.ToDictionary<int,int>(rowHelper.ReadNextCol()); //每个指令时长
            data.stageTime = CsvUtility.ToInt(rowHelper.ReadNextCol()); //关卡的时间上限
            data.nextStageName = CsvUtility.ToString(rowHelper.ReadNextCol()); //下一关的key
            return data;
        }
    }
}