# Pandora 通用 TEngine 改动同步到个人 fork

- ID: 20261015-pandora-core-sync
- 交付阶段: 通用源码已移植并完成差异检查；目标工程运行验证阻塞。

## 目标与授权

用户请求：`/Users/akiya/Project/TEngine/ 把我对tengine的修改同步到我的tengine fork`。
随后明确要求按功能拆分提交，资源加载和 JSON HTTP 分别提交，任务记录随各功能更新。
目标为 `isolamenter/TEngine` 的 `main`，允许提交并普通推送本次通用框架改动。
源码来自 Pandora `55aeeac`，目标基线为 `c06820cb92c5bde03e7d7b30b1b0e1427d33d388`。

## 范围与决策

- 同步 `ResourceExtComponent.SubSprite.cs`：支持 SpriteAtlas、共用加载句柄、等待中的请求计数、重复绑定及 Sprite 克隆释放。
- 同步 `Utility.Http.Json.cs` 及源仓库已有 `.meta`，将 `Utility.Http` 声明改为 partial：保留 HTTP 错误响应，区分调用方取消与实时超时。
- 保留目标已有的 `SetSpriteExtensions` 判空和 `ResourceExtComponent.Resource` 取消检查；Pandora 中缺少这些较新的上游修复，不能反向覆盖。
- 不同步 Pandora 配置目录移除、业务与 SDK、技能调整、生成器 DLL 二进制差异或纯换行差异。
- 保留原 TEngine 工作区已有 `.gitignore` 修改，不包含在本次提交中。

## 检查与证据

- 远端只读核对：`git ls-remote origin refs/heads/main` 为目标基线；GitHub 仓库信息确认是 `Alex-Rachel/TEngine` 的个人 fork。
- 四个同步文件逐字节匹配 Pandora 源文件；新增 `.meta` GUID 在目标 Assets/Packages 中唯一。
- 核对目标源码中 YooAsset 的 HandleBase/AssetInfo 与 UniTask 的 CancelAfterSlim 签名，确认本次使用的成员存在。
- `git diff --check` 通过。
- 在基线隔离副本执行 `python3 -B UnityProject/.codex/scripts/workflow.py doctor`，environment 和 editor-connection 均为 blocked：缺少 `UnityProject.sln` 与 `Tools/unity.exe`。原始目标工作区也缺少这两个文件。
- 未合成解决方案、安装工具或借用其他工程的 Editor。本次未取得目标工程的完整 .NET 构建或 Unity 测试通过证据。

## 剩余风险

目标 TEngine 工程的编译及 SpriteAtlas 并发、取消、销毁释放和 HTTP 超时运行回归尚未执行；此次源码同步不构成运行验收。
