// ******************************************************************
//       /\ /|       @file       CfgBattleUnitPeopleText.cs
//       \ V/        @brief      excel数据解析(由python自动生成) ./xlsx//BattleUnitPeopleText.xlsx
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
    public class RowCfgBattleUnitPeopleText
    {
        public string key; //key
        public List<string> textList; //预设路径
    }

    public class CfgBattleUnitPeopleText
    {
        private readonly Dictionary<string, RowCfgBattleUnitPeopleText> _configs = new Dictionary<string, RowCfgBattleUnitPeopleText>(); //cfgId映射row
        public RowCfgBattleUnitPeopleText this[string key] => _configs.ContainsKey(key) ? _configs[key] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{key}");
        public RowCfgBattleUnitPeopleText this[int id] => _configs.ContainsKey(id.ToString()) ? _configs[id.ToString()] : throw new Exception($"找不到配置 Cfg:{GetType()} key:{id}");
        public List<RowCfgBattleUnitPeopleText> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgBattleUnitPeopleText Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgBattleUnitPeopleText Find(string i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgBattleUnitPeopleText.txt", 3);
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
        private RowCfgBattleUnitPeopleText ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 2)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgBattleUnitPeopleText();
            var rowHelper = new RowHelper(col);
            data.key = CsvUtility.ToString(rowHelper.ReadNextCol()); //key
            data.textList = CsvUtility.ToList<string>(rowHelper.ReadNextCol()); //预设路径
            return data;
        }
    }
}