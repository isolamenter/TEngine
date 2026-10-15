# Pandora 通用 TEngine 改动同步到个人 fork

- ID: 20261015-pandora-core-sync
- 交付阶段: SpriteAtlas 源码已移植并完成差异检查；JSON HTTP 功能将在独立提交同步。目标工程运行验证阻塞。

## 目标与授权

用户请求将 Pandora 对 TEngine 的修改同步到 `/Users/akiya/Project/TEngine/` 及个人 fork，并按功能拆分提交。
目标为 `isolamenter/TEngine` 的 `main`，源码来自 Pandora `55aeeac`，目标基线为 `c06820cb92c5bde03e7d7b30b1b0e1427d33d388`。

## 当前功能范围

- 同步 `ResourceExtComponent.SubSprite.cs`：支持 SpriteAtlas、共用加载句柄、等待中的请求计数、重复绑定及 Sprite 克隆释放。
- 保留目标已有的 `SetSpriteExtensions` 判空和 `ResourceExtComponent.Resource` 取消检查。
- 保留原 TEngine 工作区已有 `.gitignore` 修改，不包含在本次提交中。

## 检查与剩余风险

文件逐字节匹配 Pandora 源码，目标 YooAsset 的 HandleBase/AssetInfo 成员已核对，`git diff --check` 通过。
基线隔离副本执行 `python3 -B UnityProject/.codex/scripts/workflow.py doctor`，因缺少 `UnityProject.sln` 与 `Tools/unity.exe` 返回 blocked；原始目标工作区也缺少这两个文件。
未取得目标工程的完整 .NET 构建或 Unity 测试通过证据。SpriteAtlas 并发、取消和销毁释放运行回归尚未执行。
