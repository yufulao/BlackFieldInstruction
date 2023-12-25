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
        public int lowCount; //地图区域的数量
        public List<float> highCameraParams; //high摄像机参数
        public List<float> midCameraParams; //mid摄像机参数
        public List<float> lowACameraParams; //A区域的摄像机参数
        public List<float> lowBCameraParams; //B区域的摄像机参数
        public List<float> lowCCameraParams; //C区域的摄像机参数
        public List<float> lowDCameraParams; //D区域的摄像机参数
        public List<float> lowECameraParams; //E区域的摄像机参数
        public List<float> lowFCameraParams; //F区域的摄像机参数
        public List<float> lowGCameraParams; //G区域的摄像机参数
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
            if (col.Length < 12)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgMap();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.mapName = CsvUtility.ToString(rowHelper.ReadNextCol()); //地图名
            data.lowCount = CsvUtility.ToInt(rowHelper.ReadNextCol()); //地图区域的数量
            data.highCameraParams = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //high摄像机参数
            data.midCameraParams = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //mid摄像机参数
            data.lowACameraParams = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //A区域的摄像机参数
            data.lowBCameraParams = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //B区域的摄像机参数
            data.lowCCameraParams = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //C区域的摄像机参数
            data.lowDCameraParams = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //D区域的摄像机参数
            data.lowECameraParams = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //E区域的摄像机参数
            data.lowFCameraParams = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //F区域的摄像机参数
            data.lowGCameraParams = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //G区域的摄像机参数
            return data;
        }
    }
}