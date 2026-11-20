CrimsonCourtyard — ฉากลานปราสาทสีเลือด ขยับได้ สำหรับ Unity (2D)

ติดตั้ง
1. ลากโฟลเดอร์ CrimsonCourtyard ไปไว้ใน Assets/ (ต้องชื่อนี้ เพราะสคริปต์อ้างพาธ)
2. รอ import เสร็จ (sprite ตั้ง PPU 100 / ไม่บีบอัด ให้อัตโนมัติ)
3. เปิดซีนที่มี Main Camera แล้วกด Tools > Crimson Courtyard > Build Scene
4. กด Play

สิ่งที่ขยับ (แต่ละอย่างเป็นสคริปต์แยก ปรับในแท็บ Inspector ได้)
- CourtyardSway: ธง 4 ผืน ใบไม้ เถาไม้ (amplitude = องศา, speed)
- CourtyardGlow: แสงโคมไฟ เทียน พระจันทร์ นาฬิกา (baseAlpha, speed, amount)
- CourtyardEmbers: ประกายไฟลอยจากโคมและเทียน
- CourtyardFog: หมอกบนพื้น
- CourtyardLeaves: ใบไม้ร่วง 3 ระยะ (count = จำนวน)
- CourtyardParallax: กล้องขยับตามเมาส์/นิ้ว ปล่อยนิ่งแล้วแกว่งเอง
  (manualInput ป้อน -1..1 จากเกม เช่นตามตัวละคร)

Sorting Order: base 0 | หมอก 6 | ธง 10 | เถาไม้ 20 | ต้นไม้ 30 | แสง 40 | ประกายไฟ 41 | ใบไม้ใกล้ 55
ตัวละครให้ใส่ Order ระหว่าง 1 กับ 9 ถ้าให้เดินบนพื้นหลังแต่อยู่หลังธง

หมายเหตุ
- ใช้ URP แล้วแสงเรืองไม่ขึ้น: เปิด GlowAdditive.mat แล้วเปลี่ยน shader เป็น Particles/Additive (URP) หรือ Sprites/Default ที่ blend แบบ additive
- กล้องตั้งให้พอดีความสูงภาพ (ภาพแนวตั้ง 1024x1536) ถ้าเกมเป็นแนวนอนให้ปรับ Orthographic Size หรือเลื่อนกล้อง
- ส่วนที่ถูกบังหลังธง/ใบไม้เติมด้วย inpainting อัตโนมัติ
- สคริปต์รองรับ Input Manager เก่าและ Input System ใหม่ แต่ผมยังไม่ได้ทดสอบใน Unity จริง ถ้า error ส่งข้อความมาได้
