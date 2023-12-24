// ******************************************************************
//       /\ /|       @file       CfgMap.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//Map.xlsx
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
    public class RowCfgMap
    {
        public string key; //key
        public string mapName; //地图名
        public int areaCount; //地图区域的数量
        public List<float> cameraPosition; //摄像机位置
        public List<float> area0CameraPosition; //0区域的摄像机位置和旋转角度和深度
        public List<float> area1CameraPosition; //1区域的摄像机位置
        public List<float> area2CameraPosition; //2区域的摄像机位置
        public List<float> area3CameraPosition; //3区域的摄像机位置
        public List<float> area4CameraPosition; //4区域的摄像机位置
        public List<float> area5CameraPosition; //5区域的摄像机位置
        public List<float> area6CameraPosition; //6区域的摄像机位置
    }

    public class CfgMap
    {
        private readonly Dictionary<string, RowCfgMap> _configs = new Dictionary<string, RowCfgMap>(); //cfgId映射row
        public RowCfgMap this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgMap this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgMap> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgMap Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgMap Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgMap.txt", 3);
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
        private RowCfgMap ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 11)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgMap();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.mapName = CsvUtility.ToString(rowHelper.ReadNextCol()); //地图名
            data.areaCount = CsvUtility.ToInt(rowHelper.ReadNextCol()); //地图区域的数量
            data.cameraPosition = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //摄像机位置
            data.area0CameraPosition = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //0区域的摄像机位置和旋转角度和深度
            data.area1CameraPosition = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //1区域的摄像机位置
            data.area2CameraPosition = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //2区域的摄像机位置
            data.area3CameraPosition = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //3区域的摄像机位置
            data.area4CameraPosition = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //4区域的摄像机位置
            data.area5CameraPosition = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //5区域的摄像机位置
            data.area6CameraPosition = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //6区域的摄像机位置
            return data;
        }
    }
}