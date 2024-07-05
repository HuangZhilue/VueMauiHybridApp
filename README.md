# VueMauiHybridApp

# ʹ��.Net MAUI Blazor Hybrid����Ͱ�װVueӦ��

## ���
ʹ��.Net MAUI Blazor Hybrid ������Ͱ�װһ��VueӦ�á��Ա㽫Vue��ҳ������׿App��Window�������ȵȵ�����ƽ̨��չʾ��

## �Ⱦ�����

1.	����.Net MAUI Blazor Hybrid��Ŀ

>��Visual Studio������һ���µ�.Net MAUI Blazor Hybrid��Ŀ���������򵼵�ָʾ�����Ŀ�Ĵ�����

![����MAUI Blazor Hybrid��Ŀ](./Sample/Sample_1.png)

2.	����VueӦ��

>�����Լ�ϲ���ķ�ʽ����һ��VueӦ�á�

>���磺����vite��[�ٷ��ĵ�](https://vuejs.org/guide/quick-start.html)����һ��vueӦ�á�

![����VueӦ��](./Sample/Sample_2.png)

---

## ��ͬ�Ĳ�����ʽ

### ��򵥵ģ�����Ҫ.Net��Vue����������

��򵥣�����עJS��.Net֮��Ļ��������ģ������ǽ�һ���򵥵�vue��ҳ�����Ӧ�ó���ġ�

-	����vueӦ��

-	�򿪹�����ɵ�Ŀ¼����distĿ¼���������������ļ�

-	��MAUI Blazor Hybrid��Ŀ�е�wwwrootĿ¼����ճ�������ļ�

-	����MAUI Blazor HybridӦ�ü���

![����ճ������](./Sample/Sample_3.png)

---

### �򵥵�Vue��.Net����

�Ƚϼ򵥣���Ҫvue�������һЩ.Net�����ģ�����Ӧ��Ȩ�ޡ���������ͷ��ɨ�������롢��ά�������ġ�

-	��vue��Ŀ��index.html�ļ�����&lt;body&gt;��ǩ��������´���
    ```js
    <script src="_framework/blazor.webview.js" autostart="false"></script>
    ```
    >[!TIP]
    >Vue�����׶���ֱ�Ӻ���"�Ҳ���_framework/blazor.webview.js"�Ĵ���

-   ��.Net MAUI Blazor Hybrid��Ŀ�е�MainPage.xaml.cs������[΢��ٷ��̳�](https://learn.microsoft.com/aspnet/core/blazor/javascript-interoperability/call-dotnet-from-javascript)��дJSInvokable���������磺
    ```csharp
    [JSInvokable]
    public static async Task<string> CallDotNetFromJs(string message)
    {
        return await Task.FromResult("CallDotNetFromJs");
    }
    ```

-   ��vue��Ŀ��ͬ���ǰ���[΢��ٷ��̳�](https://learn.microsoft.com/aspnet/core/blazor/javascript-interoperability/call-dotnet-from-javascript)��дJavascript����
    ```js
    DotNet.invokeMethodAsync(
      MAUI_Project_ASSEMBLY_NAME,
      'CallDotNetFromJs',
      'this is Message'
    ).then((data) => {
      console.log(data)
    })
    ```
    > [!NOTE]  
    > �����Typescript��Ŀ����Ҫע�⣬Ҫ����ts���������"DotNet"����δ����Ĵ���
    > ����ʹ�����·�ʽ���Դ���
    ```ts
    // ./src/globals.d.ts
    declare interface Window {
      DotNet: any
    }

    // ./src/views/HomeView.vue
    // ����DotNet
    const DotNet = window.DotNet
    // ʹ��DotNet
    DotNet.invokeMethodAsync(xxx,xxx)
    ```

-   ����֮�����ļ�ճ����wwwrootĿ¼�С� [���ա���򵥵ġ���һ����](###��򵥵ģ�����Ҫ.Net��Vue����������)

---

### ������Vue��.Net����

���˵ڶ�������˵�Ĺ���֮�⣬����Ҫ.Net��������vue��js�������Ա�֪ͨvue����ģ����̨�������������֪ͨ�ȹ��ܡ�


---
