MoonlitGraveyard — ฉากสุสานใต้แสงจันทร์ ขยับได้ สำหรับ Unity 2D

ติดตั้ง (ง่ายสุด): ดับเบิลคลิกไฟล์ MoonlitGraveyard.unitypackage แล้วกด Import
  หรือเมนู Assets > Import Package > Custom Package
ติดตั้งแบบโฟลเดอร์: ลากโฟลเดอร์ MoonlitGraveyard (ตัวที่มี Sprites/Scripts อยู่ข้างใน) ไปวางบนคำว่า Assets
  ผลที่ถูกต้องคือพาธ Assets/MoonlitGraveyard/Sprites/01_sky.png

ใช้งาน
1. รอ Unity คอมไพล์เสร็จ ดู Console ว่าไม่มี error สีแดง
2. เปิดซีนที่มี Main Camera แล้วกด Tools > Moonlit Graveyard > Build Scene
3. กด Play (หน้าต่าง Game ตั้งสัดส่วนเป็น 9:16 Portrait)

สิ่งที่ขยับ
- MoonlitParallax: กล้องขยับตามเมาส์/นิ้ว ปล่อยนิ่งแล้วแกว่งเอง (manualInput ป้อน -1..1 จากเกม)
- MoonlitGlow: แสงโคมไฟ โบสถ์ เทียน พระจันทร์ กะพริบ
- MoonlitEmbers: ประกายไฟ | MoonlitFireflies: หิ่งห้อย | MoonlitBat: ค้างคาว
- MoonlitLeaves: ใบไม้ร่วง 3 ระยะ | MoonlitFog: หมอก

Sorting Order: sky 0 | ค้างคาว 3 | hill 10 | หมอก 12 | tree 30 | แสง 40 | ประกายไฟ 41 | หิ่งห้อย 42 | ใบไม้ 43/55
ตัวละครที่เดินบนเนิน ให้ตั้ง Order ระหว่าง 11 กับ 29

ถ้าแสงเรืองไม่ขึ้น (URP): เปิด GlowAdditive.mat เปลี่ยน Shader เป็น Universal Render Pipeline/Particles/Unlit แล้วตั้ง Surface Type = Transparent, Blending = Additive
