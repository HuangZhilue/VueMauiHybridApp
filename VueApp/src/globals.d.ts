// 因为ts类型检查器无法识别window.DotNet对象，因此需要在此声明
// Because ts type checker can't recognize the window.DotNet object, so we need to declare it here
declare interface Window {
  DotNet: any
  [key: string]: any
}
