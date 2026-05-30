# Player Prefs Editor

Xem, sửa, lưu, xoá PlayerPrefs trực tiếp trong Editor mà không cần chạy game.

**Menu**: `manhnd_sdk > Player Prefs Editor` · **Platform**: Windows only

---

## Giao diện

```
| [💾 Save All] [↩ Revert All] [🗑 Delete All]              |
| Search [________________________][X]                       |
| Key          | Type   | Value                 |            |
| player_name  | string | Alice                 | [💾][↩][🗑] |
| high_score   | int    | 9500                  | [💾][↩][🗑] |
| config_data  | string | {"level":5,"hp":100}  | [📋][💾][↩][🗑] |
| 3 entries                                                  |
```

- **Type** có màu phân biệt: `int` cyan · `float` magenta · `string` xanh lá
- **Nút có icon** — hover để xem tooltip (Save, Revert, Delete, Edit JSON)
- **Màu nút**: 💾 Save = xanh lá · ↩ Revert = xanh dương · 🗑 Delete = đỏ
- Dòng xen kẽ sáng/tối để dễ đọc
- String dài tự wrap nhiều dòng theo chiều ngang window

---

## Chỉnh sửa

Nhập trực tiếp vào ô **Value**. Khi giá trị thay đổi:

- Ô Value chuyển **cam** → nút 💾 / ↩ kích hoạt (trước đó bị mờ disabled)
- 💾 **Save** → ghi vào Registry ngay. Giá trị không hợp lệ (vd: "abc" cho int) sẽ không lưu, Console hiện warning
- ↩ **Revert** → huỷ thay đổi, trả lại giá trị gốc
- Kiểu dữ liệu luôn được bảo toàn — int vẫn lưu int, không chuyển sang string

---

## JSON Editor

Nếu string là **JSON hợp lệ**, nút 📋 xuất hiện bên cạnh ô value. String không phải JSON sẽ không có nút này.

Nhấn 📋 mở cửa sổ popup:

- JSON hiển thị dạng **indented** (4 spaces), font monospace, dễ đọc và sửa
- Validate realtime — nếu JSON không hợp lệ, hiện cảnh báo vàng + nút Save bị **disabled**
- 💾 **Save** → compact JSON (xoá whitespace) → trả về row chính, row chuyển cam (chưa ghi Registry)
- **Cancel** → đóng popup, không thay đổi gì
- Chỉ mở được 1 popup tại 1 thời điểm — mở popup mới sẽ đóng popup cũ

Luồng: 📋 → sửa indented → Save popup → row cam → 💾 Save row → ghi Registry

---

## Xoá

- 🗑 **Delete** trên từng dòng → xoá ngay, không hỏi xác nhận
- 🗑 **Delete All** → hộp xác nhận. Khi đang search, hộp cảnh báo rõ sẽ xoá **TẤT CẢ** entry, không chỉ kết quả đang lọc

---

## Tìm kiếm

Gõ vào ô **Search** → lọc tức thì theo Key, Value, Type. Không phân biệt hoa thường. Nhấn **X** để xoá bộ lọc.

> **Lưu ý**: Save All / Delete All tác động lên **tất cả** entry, kể cả entry đang bị ẩn bởi bộ lọc.

---

## Quick Buttons

| Nút | Khi nào disabled |
|-----|------------------|
| 💾 **Save All** / ↩ **Revert All** | Không có entry nào bị modified |
| 🗑 **Delete All** | Danh sách rỗng |

Save All lưu theo đúng kiểu gốc. Entry nào parse fail thì giữ cam + Console warning, entry hợp lệ vẫn lưu bình thường.

---

## Status Bar

`42 entries` · Khi search: `12 / 42 entries` · Khi có thay đổi: `… | 3 modified`

---

## Các trường hợp đặc biệt

- **Thay đổi bên ngoài**: Danh sách tự cập nhật mỗi frame. Key bị xoá từ bên ngoài → dirty state tự dọn
- **Save All một phần fail**: Entry hợp lệ lưu xong, entry sai type giữ cam + warning
- **Float precision**: `0.3f` có thể hiện `0.300000012` — hành vi bình thường của floating point
- **JSON sửa hỏng trong popup**: Nút Save bị disabled, phải sửa lại cho đúng hoặc Cancel
- **Không thể tạo key mới** từ tool — dùng `PlayerPrefs.SetXxx()` trong code
- **Không có Undo** cho thao tác đã Save/Delete
