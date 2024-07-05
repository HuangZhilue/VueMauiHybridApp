# VueMauiHybridApp

# 使用.Net MAUI Blazor Hybrid打包和包装Vue应用

## 简介
使用.Net MAUI Blazor Hybrid 来打包和包装一个Vue应用。以便将Vue网页带到安卓App、Window桌面程序等等的其它平台上展示。

## 先决条件

1.	创建.Net MAUI Blazor Hybrid项目

>打开Visual Studio并创建一个新的.Net MAUI Blazor Hybrid项目。并按照向导的指示完成项目的创建。

![创建MAUI Blazor Hybrid项目](./Sample/Sample_1.png)

2.	创建Vue应用

>按照自己喜欢的方式创建一个Vue应用。

>比如：跟随vite的[官方文档](https://vuejs.org/guide/quick-start.html)创建一个vue应用。

![创建Vue应用](./Sample/Sample_2.png)

---

## 不同的操作方式

### 最简单的打包方式

最简单，不关注JS与.Net之间的互动操作的，仅仅是将一个简单的vue网页打包成应用程序的。

-	构建vue应用

-	打开构建完成的目录（如dist目录），并复制所有文件

-	打开MAUI Blazor Hybrid项目中的wwwroot目录，并粘贴覆盖文件

-	运行MAUI Blazor Hybrid应用即可

![复制粘贴即可](./Sample/Sample_3.png)

---

### 简单的Vue和.Net互动

比较简单，需要vue程序调用一些.Net方法的，如检查应用权限、调用摄像头、扫描条形码、二维码等任务的。

-	打开vue项目的index.html文件，在&lt;body&gt;标签内添加以下代码
    ```js
    <script src="_framework/blazor.webview.js" autostart="false"></script>
    ```
    -   Vue开发阶段请直接忽略"找不到_framework/blazor.webview.js"的错误

-

---

### 完整的Vue和.Net互动

除了第二部分所说的功能之外，还需要.Net主动调用vue的js方法，以便通知vue程序的，如后台服务、任务的主动通知等功能。


---
