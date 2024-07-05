import{_ as s,o as a,c,a as t,w as i,S as r,b as e,D as d}from"./index-C5nlNJWm.js";const l={},_={class:"about"},p=e("h1",null,"About VueMauiHybridApp",-1),u=e("p",null," Use .NET Blazor Hybrid technology to convert Vue development pages into MAUI applications ",-1),h=e("p",null," If you don't need JS/.NET interaction, just copy and paste the files in the dist directory of the vue package to the wwwroot directory of the MAUI project ",-1),f=e("p",null," If you need to use JS/.NET interaction, you need to add the following line to the index.html of the vue project ",-1),m=e("p",null,' (During the Vue development phase, please ignore the error "Cannot find _framework/blazor.webview.js") ',-1),v=e("p",null,[e("code",null,'<script src="_framework/blazor.webview.js" autostart="false"><\/script>')],-1),w=e("p",null," After adding, you can use it in the vue project ",-1),y=e("pre",null,[e("code",null,`window.DotNet.invokeMethodAsync(
  'MAUI Assembly Name', 
  'dotnet method name',
  parameter1, parameter2, ... 
).then((data) => {
  console.log(data) 
})`)],-1);function b(A,g){const o=d,n=r;return a(),c("div",_,[t(n,{direction:"vertical"},{default:i(()=>[p,u,h,t(o),f,m,v,w,y,t(o)]),_:1})])}const j=s(l,[["render",b]]);export{j as default};
