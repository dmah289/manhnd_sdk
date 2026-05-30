# Player Prefs Editor

Tool trong Unity Editor cho phép xem, sửa, lưu, xóa PlayerPrefs trực tiếp mà không cần chạy game.

**Menu**: `manhnd_sdk > Player Prefs Editor`

**Platform**: Chỉ hỗ trợ Windows. Trên platform khác, cửa sổ hiện thông báo cảnh báo.

---

## Giao diện tổng quan

Cửa sổ gồm 5 thành phần chính từ trên xuống dưới:

```
+----------------------------------------------------------+
| Player Prefs Editor                        (Section Title)|
+----------------------------------------------------------+
| [Save All]  [Revert All]  [Delete All]    (Quick Buttons) |
+----------------------------------------------------------+
| Search [_____________________________][X]  (Thanh Search) |
+----------------------------------------------------------+
| Key         | Type   | Value          |   (Column Headers)|
+----------------------------------------------------------+
| player_name | string | Alice          | [Save][Rev][Del]  |
| high_score  | int    | 9500           | [Save][Rev][Del]  |
| volume      | float  | 0.75           | [Save][Rev][Del]  |
+----------------------------------------------------------+
| 3 entries                                  (Status Bar)   |
+----------------------------------------------------------+
```

### Các cột trong bảng dữ liệu

| Cột | Mô tả |
|-----|-------|
| **Key** | Tên của PlayerPrefs key |
| **Type** | Kiểu dữ liệu (`int`, `float`, `string`), hiển thị với màu sắc phân biệt |
| **Value** | Giá trị hiện tại, có thể sửa trực tiếp |
| **Actions** | 3 nút thao tác cho từng dòng |

### Màu sắc kiểu dữ liệu

| Kiểu | Màu |
|------|-----|
| `int` | Cyan |
| `float` | Magenta |
| `string` | Xanh lá |

### Dòng xen kẽ

Các dòng dữ liệu được tô màu nền xen kẽ (đậm hơn / nhạt hơn) để dễ phân biệt khi danh sách dài.

---

## Chỉnh sửa giá trị

### Cách sửa

Nhập trực tiếp vào ô **Value** của dòng bất kỳ. Khi giá trị khác với bản gốc:

- Nền ô Value chuyển sang **màu cam** (modified indicator)
- Nút **Save** và **Revert** của dòng đó được kích hoạt (không còn bị mờ)
- Status bar cập nhật số lượng "modified"

### Lưu (Save)

Nhấn nút **Save** (màu xanh lá) trên dòng đã sửa để ghi giá trị mới vào PlayerPrefs.

- **int**: Giá trị phải là số nguyên hợp lệ. Ví dụ: `42`, `-7`, `0`
- **float**: Giá trị phải là số thực hợp lệ. Ví dụ: `3.14`, `-0.5`, `100`
- **string**: Mọi giá trị đều hợp lệ

Nếu giá trị nhập vào không hợp lệ (ví dụ gõ "abc" vào trường int):
- Giá trị **KHÔNG** được lưu
- Ô Value vẫn giữ màu cam
- Console hiện warning: `[PlayerPrefsEditor] Cannot parse "abc" as int for key "score"`

Khi lưu thành công:
- Ô Value trở lại nền bình thường
- Nút Save và Revert chuyển sang mờ (disabled)
- Dữ liệu được ghi ngay vào Registry (không cần đợi đóng Editor)

### Huỷ thay đổi (Revert)

Nhấn nút **Revert** (màu magenta) để huỷ bỏ thay đổi chưa lưu, trả lại giá trị ban gốc.

- Chỉ kích hoạt khi dòng đang trong trạng thái modified
- Không ảnh hưởng đến dữ liệu đã lưu trước đó

### Quy tắc quan trọng

- **Kiểu dữ liệu không thay đổi**: Nếu key là `int`, chỉ có thể lưu giá trị int. Không thể chuyển đổi từ int sang string hay ngược lại thông qua tool này.
- **Save ghi trực tiếp vào Registry**: Thay đổi có hiệu lực ngay lập tức và tồn tại qua các phiên chạy game.

---

## Xoá dữ liệu

### Xoá từng key

Nhấn nút **Delete** (màu đỏ) trên dòng bất kỳ.

- Key bị xoá ngay khỏi PlayerPrefs và Registry
- Không có hộp xác nhận (vì thao tác trên từng key đơn lẻ)
- Nếu dòng đang trong trạng thái modified, trạng thái modified cũng bị xoá sạch

### Xoá tất cả (Delete All)

Nhấn nút **Delete All** (màu đỏ) ở thanh Quick Buttons.

- Luôn hiện **hộp xác nhận** trước khi xoá
- **Xoá TẤT CẢ entries**, bất kể có đang lọc tìm kiếm hay không
- Khi đang search, hộp xác nhận ghi rõ: *"This will delete ALL N entries, not just the filtered results."*
- Khi không search: *"Delete all N entries? This cannot be undone."*
- Sau khi xoá, mọi trạng thái modified đều bị xoá sạch

---

## Tìm kiếm (Search)

### Cách dùng

Gõ từ khoá vào ô **Search** trên thanh toolbar.

### Phạm vi tìm kiếm

Tìm kiếm lọc dòng theo 3 trường:

1. **Key** — tên của PlayerPrefs key
2. **Value** — giá trị (dưới dạng text)
3. **Type** — kiểu dữ liệu (`int`, `float`, `string`)

### Đặc điểm

- **Không phân biệt hoa thường** (case-insensitive): gõ "score" sẽ tìm thấy "HighScore", "SCORE", "player_score"
- **Lọc tức thì**: Kết quả cập nhật ngay khi gõ, không cần nhấn Enter
- **Nút X**: Xuất hiện khi ô search có nội dung. Nhấn để xoá bộ lọc nhanh
- Status bar hiển thị số dòng hiển thị / tổng số: ví dụ `12 / 42 entries`

### Lưu ý khi search đang hoạt động

- **Save All** vẫn lưu **tất cả** entry đã modified, kể cả những entry đang bị ẩn bởi bộ lọc
- **Delete All** vẫn xoá **tất cả** entry, kể cả những entry đang bị ẩn bởi bộ lọc (hộp xác nhận sẽ cảnh báo điều này)

---

## Các nút Quick Buttons

| Nút | Màu | Chức năng | Trạng thái disabled |
|-----|-----|-----------|---------------------|
| **Save All** | Xanh lá | Lưu tất cả entry đã modified | Khi không có entry nào bị modified |
| **Revert All** | Magenta | Huỷ bỏ tất cả thay đổi chưa lưu | Khi không có entry nào bị modified |
| **Delete All** | Đỏ | Xoá tất cả PlayerPrefs (có xác nhận) | Khi không có entry nào trong danh sách |

### Save All chi tiết

- Duyệt qua **tất cả** entry đã modified (kể cả những entry bị ẩn bởi search filter)
- Mỗi entry được lưu theo đúng kiểu dữ liệu gốc (int lưu int, float lưu float, string lưu string)
- Entry nào lưu thành công thì trở lại trạng thái bình thường
- Entry nào lưu thất bại (ví dụ: giá trị không hợp lệ) thì vẫn giữ trạng thái modified, Console hiện warning
- Chỉ ghi vào Registry khi có ít nhất 1 entry lưu thành công

---

## Status Bar

Thanh trạng thái ở cuối cửa sổ, hiển thị:

| Trường hợp | Hiển thị |
|------------|----------|
| Bình thường | `42 entries` |
| Đang search | `12 / 42 entries` (12 kết quả / 42 tổng) |
| Có thay đổi chưa lưu | `42 entries  \|  3 modified` |
| Search + có thay đổi | `12 / 42 entries  \|  3 modified` |

Số "modified" đếm **tất cả** entry đã thay đổi, kể cả những entry đang bị ẩn bởi search filter.

---

## Trạng thái rỗng (Empty States)

| Trường hợp | Thông báo |
|------------|-----------|
| Không có PlayerPrefs nào | *"No PlayerPrefs found for this project."* |
| Search không có kết quả | *"No entries match the search filter."* |

---

## Các trường hợp đặc biệt

### Thay đổi từ bên ngoài

Nếu game code hoặc tool khác xoá/thêm PlayerPrefs key trong khi cửa sổ đang mở:

- Danh sách tự động cập nhật (đọc lại Registry mỗi frame)
- Nếu một key đã bị modified trong Editor nhưng bị xoá từ bên ngoài, trạng thái modified sẽ tự động bị dọn dẹp — không để lại "ghost entry"

### Nhiều thay đổi chưa lưu

Có thể sửa nhiều entry cùng lúc trước khi lưu:

1. Sửa "score" thành "100" — chuyển cam
2. Sửa "volume" thành "0.8" — chuyển cam
3. Sửa "name" thành "Bob" — chuyển cam
4. Nhấn **Save All** để lưu cả 3 (nếu hợp lệ)
5. Hoặc nhấn **Revert All** để huỷ cả 3, quay lại giá trị gốc

### Lưu thất bại một phần

Khi Save All với nhiều entry modified:

- Entry hợp lệ được lưu thành công, trở lại bình thường
- Entry không hợp lệ giữ nguyên trạng thái modified, Console hiện warning
- Chỉ những entry thành công được ghi vào Registry

Ví dụ: Sửa 3 entry, 2 hợp lệ + 1 gõ sai kiểu dữ liệu. Kết quả: 2 được lưu, 1 vẫn cam, Console hiện 1 warning.

### Float hiển thị

Giá trị float hiển thị đúng như `ToString()` trả về. Ví dụ float `0.3f` có thể hiển thị là `0.3` hoặc `0.300000012` tuỳ theo độ chính xác của floating point. Đây là hành vi bình thường của Unity PlayerPrefs.

---

## Lưu ý quan trọng

1. **Tool này chỉ đọc/ghi PlayerPrefs** — không phải EditorPrefs. Để chỉnh sửa EditorPrefs, cần tool riêng.

2. **Thay đổi là vĩnh viễn** — Khi nhấn Save hoặc Delete, dữ liệu được ghi trực tiếp vào Windows Registry. Không có nút Undo cho thao tác đã ghi.

3. **Kiểu dữ liệu được bảo toàn** — Nếu key "score" là int, dù gõ "100" (nhìn như string), tool vẫn lưu đúng kiểu int thông qua `PlayerPrefs.SetInt`.

4. **Không thể tạo key mới** — Tool chỉ cho phép xem và sửa key đã tồn tại. Để tạo key mới, dùng `PlayerPrefs.SetXxx()` trong code.

5. **Độ rộng cột theo tỷ lệ** — Các cột tự động điều chỉnh theo chiều rộng cửa sổ (25% Key, 10% Type, 40% Value, phần còn lại cho Actions).
