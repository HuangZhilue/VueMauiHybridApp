<template>
  <div class="about">
    <van-space direction="vertical">
      <h1>About VueMauiHybridApp</h1>

      <!-- <p>使用.Net Blazor Hybrid技术将Vue开发的页面变成MAUI应用</p> -->
      <p>
        Use .NET Blazor Hybrid technology to convert Vue development pages into MAUI applications
      </p>
      <!-- <P
        >如果不需要JS/.NET的互操作,
        仅需复制粘贴vue打包的dist目录的文件到MAUI项目的wwwroot目录中覆盖即可</P
      > -->
      <p>
        If you don't need JS/.NET interaction, just copy and paste the files in the dist directory
        of the vue package to the wwwroot directory of the MAUI project
      </p>
      <van-divider />
      <!-- <p>如果需要使用JS/.NET互操作特性,需要在vue项目的index.html中额外引入下面这俩行</p> -->
      <p>
        If you need to use JS/.NET interaction features, you need to add the following two lines to
        the index.html of the vue project
      </p>
      <!-- <p>
        (Vue开发阶段请直接忽略"找不到_framework/blazor.webview.js"的错误)
      </p> -->
      <p>
        (During the Vue development phase, please ignore the error "Cannot find
        _framework/blazor.webview.js")
      </p>
      <pre>
        <code
          >&lt;div id="blazorapp"&gt;&lt;/div&gt; 
&lt;script src="_framework/blazor.webview.js" autostart="false"&gt;&lt;/script&gt;</code
        ></pre>
      <p>
        <!-- 添加之后, 即可在vue项目中使用 -->
        After adding, you can use it in the vue project
      </p>
      <pre><code>window.DotNet.invokeMethodAsync(
  'MAUI Assembly Name', 
  'dotnet method name',
  parameter1, parameter2, ... 
).then((data) => {
  console.log(data) 
})</code></pre>
      <van-divider />
    </van-space>
  </div>
</template>

<style>
code {
  display: block;
  padding: 10px;
  border: 1px solid #ccc;
  font-family: Consolas, monospace;
  white-space: pre;
}
@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
