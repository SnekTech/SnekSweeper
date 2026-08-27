using FileAccess = System.IO.FileAccess;

namespace SnekGameDevKit;

/// <summary>
/// 通用文件 I/O 工具（从 GodotGadgets 迁移而来，纯 .NET，无 Godot 依赖）。
/// </summary>
public static class FileOperations
{
    public static async Task SafeWriteAllTextAsync(string path, string contents, CancellationToken ct = default)
    {
        // 1. 解析为绝对路径：临时文件与目标同目录（同卷才能保证 File.Move 原子替换）
        var fullPath = Path.GetFullPath(path);
        var tempPath = Path.Combine(
            Path.GetDirectoryName(fullPath)!,
            $"{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            // 2. 将所有内容写入临时文件
            await File.WriteAllTextAsync(tempPath, contents, ct);

            // 3. 确保数据完全写入磁盘
            await using (var fs = new FileStream(tempPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096,
                             true))
            {
                await fs.FlushAsync(ct);
            }

            // 4. 关键操作：用临时文件原子地替换目标文件
            // 此操作在大多数操作系统上是原子的
            File.Move(tempPath, fullPath, overwrite: true);
        }
        finally
        {
            // 5. 清理工作：如果临时文件还存在（例如在替换前就被取消了），就删除它
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch
                {
                    /* 记录日志，但不要抛出异常 */
                }
            }
        }
    }
}
