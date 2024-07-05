<template>
  <main>
    <van-space direction="vertical">
      <van-divider />
      <van-button @click="DisplayAlert()" type="primary">DisplayAlert</van-button>

      <van-button @click="ToastMake()" type="primary">ShowToast</van-button>
      <van-button @click="CheckPermissionStatus()" type="primary">
        Check Permission Status (Camera, StorageRead, StorageWrite)
      </van-button>
      <van-button @click="RequestPermission()" type="primary">
        Request Permission (Camera, StorageRead, StorageWrite)
      </van-button>

      <div v-for="(status, key) in PermissionStatus" :key="key">
        <p>{{ key }}: {{ status ? '✔' : '✖' }}</p>
      </div>
      <van-divider />
      <!-- 重写blazorwebview相关方法所实现的拍照、选图功能 -->
      <!-- Rewriting the blazorwebview related methods to implement the camera and picture selection functions -->
      <van-uploader v-model="fileList" multiple />
      <div>Rewrite BlazorWebView to implement &lt;input type="file" capture="camera" /&gt;</div>
      <div>
        For more details, see
        <a href="https://github.com/dotnet/maui/issues/884#issuecomment-1760299780" target="blank"
          >https://github.com/dotnet/maui/issues/884#issuecomment-1760299780</a
        >
      </div>
      <van-divider />
      <van-button @click="CapturePhoto()" plain type="primary">Capture Photo</van-button>
      <van-button @click="TakePhoto()" plain type="primary">Take Photo</van-button>
      <van-space>
        <van-image
          v-for="(file, index) in fileList2"
          :key="index"
          width="100"
          height="100"
          :src="file"
        />
      </van-space>
      <div>Use the simplest photo taking and image selection API provided by .Net</div>
      <div>
        For more details, see
        <a
          href="https://learn.microsoft.com/dotnet/maui/platform-integration/device-media/picker"
          target="blank"
          >https://learn.microsoft.com/dotnet/maui/platform-integration/device-media/picker</a
        >
      </div>
      <van-divider />
      <van-button @click="JsCallDotNetCallJs()" plain type="primary"
        >.Net Invoke Js Function</van-button
      >
      <p>
        For demo, this way is used: JS first calls .Net, which triggers the related events in .Net
        to trigger the JSRuntime to call JS
      </p>
      <van-divider />
    </van-space>
  </main>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'

const ASSEMBLY_NAME = 'MauiHybridApp'
// 因为ts类型检查的需要，所以要在这里先定义。如果是js，可直接使用DotNet对象而不需要指定为window.DotNet
// Because ts type checker requires it, so we need to define it here. If it's js, we can directly use the DotNet object
const DotNet = window.DotNet

const PermissionStatus = ref<{ [key: string]: boolean }>({
  Camera: false,
  StorageRead: false,
  StorageWrite: false
})
const fileList = ref([])
const fileList2 = ref<string[]>([])

onMounted(() => {
  // TODO 暴露js方法到window中，以便.Net调用JS
  // TODO Expose js methods to window for .Net to call JS
  window['JsAlert'] = (data: string) => {
    console.log('JsAlert:\t' + data)
    alert(data)
    return 'JsAlert Success'
  }
})

const DisplayAlert = () => {
  console.log('DisplayAlert')
  if (typeof DotNet !== 'undefined' && DotNet !== null) {
    // string title, string message, string cancel, string accept = null!
    DotNet.invokeMethodAsync(
      ASSEMBLY_NAME,
      'DisplayAlertAsync',
      'this is Title',
      'this is Message',
      'this is Cancel',
      'this is Accept(can be null)'
    ).then((data: unknown) => {
      console.log(data)
    })
  }
}

const ToastMake = () => {
  console.log('ToastMake')
  if (typeof DotNet !== 'undefined' && DotNet !== null) {
    // string message
    DotNet.invokeMethodAsync(ASSEMBLY_NAME, 'ToastMake', 'this is Toast Message')
  }
}

const CheckPermissionStatus = () => {
  console.log('CheckPermissionStatus')
  if (typeof DotNet !== 'undefined' && DotNet !== null) {
    const list = ['Camera', 'StorageRead', 'StorageWrite']
    try {
      const promises = list.map((key) => {
        return DotNet.invokeMethodAsync(ASSEMBLY_NAME, 'CheckPermissionStatusAsync', key).then(
          (result: unknown) => {
            console.log(key + '\t:\t' + result)
            PermissionStatus.value[key] = result === 'Granted'
          }
        )
      })

      Promise.all(promises)
    } catch (err) {
      console.warn(err)
    }
  }
}

const RequestPermission = () => {
  console.log('RequestPermission')
  if (typeof DotNet !== 'undefined' && DotNet !== null) {
    const list = ['Camera', 'StorageRead', 'StorageWrite']
    try {
      const promises = list.map((key) => {
        return DotNet.invokeMethodAsync(ASSEMBLY_NAME, 'RequestPermissionAsync', key).then(
          (result: unknown) => {
            console.log(key + '\t:\t' + result)
            PermissionStatus.value[key] = result === 'Granted'
          }
        )
      })

      Promise.all(promises)
    } catch (err) {
      console.warn(err)
    }
  }
}

const CapturePhoto = () => {
  console.log('RequestPermission')
  if (typeof DotNet !== 'undefined' && DotNet !== null) {
    DotNet.invokeMethodAsync(ASSEMBLY_NAME, 'CapturePhoto').then((data: string) => {
      console.log(data)
      fileList2.value.push(data)
    })
  }
}

const TakePhoto = () => {
  console.log('RequestPermission')
  if (typeof DotNet !== 'undefined' && DotNet !== null) {
    DotNet.invokeMethodAsync(ASSEMBLY_NAME, 'TakePhoto').then((data: string) => {
      console.log(data)
      fileList2.value.push(data)
    })
  }
}

const JsCallDotNetCallJs = () => {
  console.log('JsCallDotNetCallJs')
  if (typeof DotNet !== 'undefined' && DotNet !== null) {
    DotNet.invokeMethodAsync(ASSEMBLY_NAME, 'JsCallDotNetCallJs').then((data: string) => {
      console.log('JsCallDotNetCallJs:\t' + data)
    })
  }
}
</script>
