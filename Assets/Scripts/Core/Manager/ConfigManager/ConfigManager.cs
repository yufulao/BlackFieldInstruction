// ******************************************************************
//       /\ /|       @file       ConfigManager.cs
//       \ V/        @brief      配置表管理器(由python自动生成)
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |
//      /  \\        @Modified   2022-04-23 22:40:15
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

namespace Rabi
{
    public sealed class ConfigManager : BaseSingleTon<ConfigManager>
    {
        public readonly CfgBattleUnitPeopleText cfgBattleUnitPeopleText = new CfgBattleUnitPeopleText();
        public readonly CfgBgm cfgBgm = new CfgBgm();
        public readonly CfgCamera cfgCamera = new CfgCamera();
        public readonly CfgCommand cfgCommand = new CfgCommand();
        public readonly CfgMap cfgMap = new CfgMap();
        public readonly CfgPrefab cfgPrefab = new CfgPrefab();
        public readonly CfgSfx cfgSfx = new CfgSfx();
        public readonly CfgSprite cfgSprite = new CfgSprite();
        public readonly CfgStage cfgStage = new CfgStage();
        public readonly CfgUI cfgUI = new CfgUI();

        public ConfigManager()
        {
            //初始场景有Text的情况 查找翻译文本需要加载资源 因为同为Awake回调 加载顺序可能优于AssetManager 故补充加载
            AssetManager.Instance.OnInit();
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            cfgBattleUnitPeopleText.Load();
            cfgBgm.Load();
            cfgCamera.Load();
            cfgCommand.Load();
            cfgMap.Load();
            cfgPrefab.Load();
            cfgSfx.Load();
            cfgSprite.Load();
            cfgStage.Load();
            cfgUI.Load();
        }
    }}