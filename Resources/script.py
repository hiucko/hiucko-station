import os
from PIL import Image

def process_image(image_path):
    # Открываем изображение
    img = Image.open(image_path).convert("RGBA")
    pixels = img.load()

    # Размеры изображения
    width, height = img.size

    # Создаем копию изображения для результата
    result = img.copy()
    result_pixels = result.load()

    # Проходим по каждому пикселю
    for x in range(width):
        for y in range(height):
            r, g, b, a = pixels[x, y]

            # Если пиксель полностью прозрачный, пропускаем его
            if a == 0:
                continue

            # Проверяем, граничит ли пиксель с краем изображения или с полностью прозрачным пикселем
            borders_edge_or_full_transparency = False

            # Проверяем соседние пиксели (только по горизонтали и вертикали)
            for dx, dy in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
                nx, ny = x + dx, y + dy
                if nx < 0 or nx >= width or ny < 0 or ny >= height:
                    borders_edge_or_full_transparency = True  # Граничит с краем изображения
                    break
                _, _, _, na = pixels[nx, ny]
                if na == 0:  # Граничит с полностью прозрачным пикселем
                    borders_edge_or_full_transparency = True
                    break

            # Применяем правила перекрашивания
            if borders_edge_or_full_transparency:
                result_pixels[x, y] = (0, 0, 0, a)  # Чёрный, сохраняем исходную прозрачность
            else:
                result_pixels[x, y] = (255, 255, 255, a)  # Белый, сохраняем исходную прозрачность

    # Сохраняем результат, перезаписывая исходное изображение
    result.save(image_path)
    print(f"Processed and overwritten: {image_path}")

def process_folder(folder_path):
    # Рекурсивно обходим все подпапки
    for root, _, files in os.walk(folder_path):
        for filename in files:
            if filename.endswith(".png"):
                image_path = os.path.join(root, filename)
                process_image(image_path)

if __name__ == "__main__":
    folder_path = "Textures"  # Укажите путь к вашей папке
    process_folder(folder_path)