# Remote Config System

Quản lý biến cấu hình từ xa, hỗ trợ fetch từ server, cache offline, fallback về mặc định.

## Thứ tự ưu tiên lấy giá trị

Khi `allowFetching = true`:

```
1. Remote Provider  → fetch thành công + parse OK → dùng, cache vào PlayerPrefs
2. PlayerPrefs      → có cache từ lần trước + parse OK → dùng
3. Editor Default   → giữ giá trị set trên Inspector
```

Khi `allowFetching = false`: luôn dùng giá trị Editor, bỏ qua remote.

Giá trị chỉ cache vào PlayerPrefs khi parse thành công. Remote trả sai format sẽ KHÔNG ghi đè cache.

## Kiểu hỗ trợ

`string`, `int`, `long`, `bool`, `float`, `double` (InvariantCulture), `enum` (tên hoặc số), kiểu khác qua `JsonConvert`.

## Cách dùng

```csharp
// 1. Khai báo biến trong partial class
public partial class RCVariableCollection
{
    [RegisteredRCVar]
    [SerializeField] private RCVariable<int> maxRetryCount;

    public RCVariable<int> MaxRetryCount => maxRetryCount;
}

// 2. Khởi tạo
RCVariableCollection.Instance.Initialize();

// 3. Đọc giá trị (implicit operator hoặc .Value)
int max = RCVariableCollection.Instance.MaxRetryCount;
```

## Inspector (mỗi RCVariable)

- **firebaseKey** — Key trên Remote Config server
- **allowFetching** — Bật/tắt fetch từ remote
- **value** — Giá trị mặc định

## Editor Tools

| Tool | Vị trí | Chức năng |
|------|--------|-----------|
| CopyJsonToClipboard | RCVariable | Copy giá trị hiện tại thành JSON |
| ImportDefaultValue | RCVariable | Import giá trị mặc định từ string |
| SearchFirebaseKeyUsage | RCVariableCollection | Tìm field đang dùng một firebase key |
| EnableAllFetching | RCVariableCollection | Bật `allowFetching = true` cho tất cả biến |
