using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FairyGUI;
//using GJ_UIPackage;
using FairyGUI.Utils;
using UnityEditor.PackageManager.UI;


public class BasicUIMgr
{
    public int panelType;
    public Vector2 resolution;
    public Vector2 targetResolution;
    public string pathHead;
    private static BasicUIMgr instance = new BasicUIMgr();
    public static BasicUIMgr Instance => instance;
    //创建单例模式,防止多次生成
    //注意单例模式类通过UIManager.Instance来访问

    private Dictionary<string, GComponent> panelDic = new Dictionary<string, GComponent>();
    private Dictionary<string,FairyGUI.Window>winDic = new Dictionary<string, FairyGUI.Window>();

        public BasicUIMgr()
        {
            resolution = new Vector2(1920, 1080);
            targetResolution = new Vector2(1920, 1080);
            pathHead = "UI/PublishedUI/";
            GRoot.inst.SetContentScaleFactor((int)this.targetResolution.x, (int)this.targetResolution.y, UIContentScaler.ScreenMatchMode.MatchHeight);
                //初始化面板
                //包括其他初始化例如设置字体相关的内容都可以在此定义

             //UIObjectFactory.SetPackageItemExtension("ui://GamePackage/Panel", typeof(UI_mainPanelTap));
                //注册需要使用的面板的相关代码(详见组件拓展类相关)
        }

        //显示面板方法,规定组件名和面板类名一致
        public T ShowPanel<T>(string PackageName, string PanelName) where T : GComponent
        {
            //如果字典中已存储则直接返回
            if (panelDic.ContainsKey(PanelName))
            {
                //先激活再返回
                panelDic[PanelName].visible = true;
                return panelDic[PanelName] as T;
            }

            //Type panelType = typeof(T);
            //string panelName = panelType.Name;
            //获取对应的组件类型，此方法适用于组件名和脚本名相同的情况

            UIPackage package = UIPackage.AddPackage(pathHead + PackageName);
            foreach (var item in package.dependencies)
            {
                UIPackage.AddPackage(item["name"]);
            }
            //加载包及其依赖包,传入已加载的包也没有问题

            GComponent panel = UIPackage.CreateObject(PackageName, PanelName).asCom;
            panel.MakeFullScreen();
            GRoot.inst.AddChild(panel);
            //建立宽高关联,自适应屏幕大小
            panel.AddRelation(GRoot.inst, RelationType.Size);
            //存储面板
            panelDic.Add(PanelName, panel);
            //将父类转子类
            return panel as T;
        }

        //隐藏面板方法,给一个默认为flase(不手动传参时)的参数
        public void HidePanel<T>(string panelName,bool isDispose = false) where T : GComponent
        {
            Type panelType = typeof(T);
            if (!panelDic.ContainsKey(panelName))
            {
                return;
            }//如果已经不存在则直接结束

            if (isDispose)
            {
                panelDic[panelName].Dispose();
                panelDic.Remove(panelName);
            }//如果想移除面板
            else
            {
                panelDic[panelName].visible = false;
            }
            //节约内存用删除,节约性能用隐藏
        }

        public T GetPanel<T>(string panelName) where T : GComponent
        {
            if (panelDic.ContainsKey(panelName))
            {
                return panelDic[panelName] as T;
            }
            return null;
            //找到则返回,找不到返回null
        }

        //清空面板,过场景时执行
        public void ClearPanel(bool isGC = false)
        {
            //清空场景上的面板
            foreach (var item in panelDic.Values)
            {
                item.Dispose();
            }
            panelDic.Clear();

            //删除包,内存资源回收
            if (isGC)
            {
                UIPackage.RemoveAllPackages();
                GC.Collect();
            }
        }

    //以下是管理窗口代码
    public T ShowWindow<T>(string PackageName, string WindowName) where T : FairyGUI.Window, new()
    {
        Type type = typeof(T);

        if (winDic.ContainsKey(WindowName))
        {
            //先激活再返回
            winDic[WindowName].Show();
            return winDic[WindowName] as T;
        }
        FairyGUI.Window win = new T();

        //以下框起部分在T类(窗口类)的OnInit()即构造函数中如果已经写了则不需要添加
        win.contentPane = UIPackage.CreateObject(PackageName, WindowName).asCom;
        win.MakeFullScreen();
        win.contentPane.MakeFullScreen(); //设置自适应(可选)
        //以上框起部分在T类(窗口类)的OnInit()即构造函数中如果已经写了则不需要添加
        win.AddRelation(GRoot.inst, RelationType.Size);

        winDic.Add(WindowName, win);
        win.Show();
        return win as T;
    }

    public void HideWindow<T>(bool isDispose,string windowName) where T : FairyGUI.Window
    {
        if (!winDic.ContainsKey(windowName))
        {
            return;
        }//如果已经不存在则直接结束

        if (isDispose)
        {
            winDic[windowName].Dispose();
            winDic.Remove(windowName);
        }//如果想移除面板
        else
        {
            winDic[windowName].Hide();
        }
    }

    public T GetWin<T>(string windowName) where T : FairyGUI.Window
    {
        if (winDic.ContainsKey(windowName))
        {
            return winDic[windowName] as T;
        }
        return null;
        //找到则返回,找不到返回null
    }

    public void ClearWin(bool isGC = false)
    {
        //清空场景上的面板
        foreach (var item in winDic.Values)
        {
            item.Dispose();
        }
        winDic.Clear();

        //删除包,内存资源回收
        if (isGC)
        {
            UIPackage.RemoveAllPackages();
            GC.Collect();
        }
    }
}
