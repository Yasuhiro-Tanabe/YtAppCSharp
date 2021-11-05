
-- 連番管理
delete from SEQUENCES;
insert into SEQUENCES values ( 'SEQ_CUSTOMERS', 7 );
insert into SEQUENCES values ( 'SEQ_SHIPPING', 19 );
insert into SEQUENCES values ( 'SEQ_SUPPLIERS', 2 );
insert into SEQUENCES values ( 'SEQ_SESSION', 0 );
insert into SEQUENCES values ( 'SEQ_INVENTORY_LOT_NUMBER', 19 );


-- 得意先
delete from CUSTOMERS;
insert into CUSTOMERS values ( 1, 'ysoga@localdomain', "蘇我幸恵", 'sogayukie12345', '9876543210123210', 0 );
insert into CUSTOMERS values ( 2, 'user2@localdomain', "ユーザ2", 'user2', '1234123412341230', 0 );
insert into CUSTOMERS values ( 3, 'user3@localdomain', "ユーザ3", 'user3', '1234567890123450', 0 );

-- お届け先
delete from SHIPPING_ADDRESS;
insert into SHIPPING_ADDRESS values ( 1, 1, "東京都中央区京橋1-10-7", "KPP八重洲ビル10階", "ピアノ生徒1", 20210101 );
insert into SHIPPING_ADDRESS values ( 2, 1, "茨城県水戸市城南2-1-20", "井門水戸ビル5階", "ピアノ生徒2", 20210101 );
insert into SHIPPING_ADDRESS values ( 3, 1, "静岡県浜松市中区板屋町111-2", "浜松アクトタワー10階", "友人A", 20210101 );
insert into SHIPPING_ADDRESS values ( 4, 1, "愛知県名古屋市西区上名古屋3-25-28", "第7猪村ビル4階", "ピアノ生徒3", 20210101 );
insert into SHIPPING_ADDRESS values ( 5, 1, "大阪府吹田市垂水町3-9-10", "白川ビル3階", "ピアノ生徒4", 20210101 );
insert into SHIPPING_ADDRESS values ( 6, 1, "福岡県福岡市博多区博多駅南2-8-16", "東洋マンション駅南スターオフィス2階", "友人B", 20210101 );

insert into SHIPPING_ADDRESS values ( 7, 2, "住所2-1", '', "友人A", 20210101 );
insert into SHIPPING_ADDRESS values ( 8, 2, "住所2-2", "建物2A", "友人B", 20210101 );
insert into SHIPPING_ADDRESS values ( 9, 2, "住所2-3", '', "友人C", 20210101 );
insert into SHIPPING_ADDRESS values ( 10, 2, "住所2-4", '', "生徒1", 20210101 );
insert into SHIPPING_ADDRESS values ( 11, 2, "住所2-5", "建物2B", "生徒2", 20210101 );
insert into SHIPPING_ADDRESS values ( 12, 2, "住所2-6", '', "生徒3", 20210101 );
insert into SHIPPING_ADDRESS values ( 13, 2, "住所2-7", '', "生徒4", 20210101 );

insert into SHIPPING_ADDRESS values ( 14, 3, "住所3-1-1", '', "友人V", 20210101 );
insert into SHIPPING_ADDRESS values ( 15, 3, "住所3-1-2", '', "友人W", 20210101 );
insert into SHIPPING_ADDRESS values ( 16, 3, "住所3-1-3", '', "友人X", 20210101 );
insert into SHIPPING_ADDRESS values ( 17, 3, "住所3-1-4", '', "友人Y", 20210101 );
insert into SHIPPING_ADDRESS values ( 18, 3, "住所3-1-5", '', "友人Z", 20210101 );
insert into SHIPPING_ADDRESS values ( 19, 3, "住所3-1-6", '', "親戚1", 20210101 );

-- 仕入れ先
delete from SUPPLIERS;
insert into SUPPLIERS values ( 1, "新橋園芸", "東京都中央区銀座", "銀座六丁目園芸団地21-8", '00012345678', '00012345677', 'shinbashi@localdomain' );
insert into SUPPLIERS values ( 2, "木挽町花壇", "東京都中央区銀座四丁目121-5", '', '09098765432', '0120000000', 'kobiki@localdomain' );


-- 単品
delete from BOUQUET_PARTS;
insert into BOUQUET_PARTS values ( 'BA001', "薔薇(赤)", 1, 100, 3, 0 );
insert into BOUQUET_PARTS values ( 'BA002', "薔薇(白)", 1, 100, 3, 0 );
insert into BOUQUET_PARTS values ( 'BA003', "薔薇(ピンク)", 1, 100, 3, 0 );
insert into BOUQUET_PARTS values ( 'GP001', "かすみ草", 2, 50, 2, 0 );
insert into BOUQUET_PARTS values ( 'CN001', "カーネーション(赤)", 3, 50, 5, 0 );
insert into BOUQUET_PARTS values ( 'CN002', "カーネーション(ピンク)", 3, 50, 5, 0 );

-- 単品仕入れ先
delete from BOUQUET_SUPPLIERS;
insert into BOUQUET_SUPPLIERS values ( 1, 'BA001' );
insert into BOUQUET_SUPPLIERS values ( 1, 'BA002' );
insert into BOUQUET_SUPPLIERS values ( 1, 'BA003' );
insert into BOUQUET_SUPPLIERS values ( 1, 'GP001' );
insert into BOUQUET_SUPPLIERS values ( 2, 'GP001' );
insert into BOUQUET_SUPPLIERS values ( 2, 'CN001' );
insert into BOUQUET_SUPPLIERS values ( 2, 'CN002' );


-- 商品(花束セット)
delete from BOUQUET_SET;
insert into BOUQUET_SET values ( 'HT001', "花束-Aセット", '', 1, 0 );
insert into BOUQUET_SET values ( 'HT002', "花束-Bセット", '', 2, 0 );
insert into BOUQUET_SET values ( 'HT003', "一輪挿し-A", '', 1, 0 );
insert into BOUQUET_SET values ( 'HT004', "結婚式用ブーケ", '', 5, 0 );
insert into BOUQUET_SET values ( 'HT005', "母の日感謝セット", '', 5, 0 );
insert into BOUQUET_SET values ( 'HT006', "花束-Cセット", '', 5, 0 );
insert into BOUQUET_SET values ( 'HT007', "還暦祝い60本セット", '', 1, 0 );

-- 商品構成
delete from BOUQUET_PARTS_LIST;
insert into BOUQUET_PARTS_LIST values ( 'HT001', 'BA001', 4 );

insert into BOUQUET_PARTS_LIST values ( 'HT002', 'BA001', 3 );
insert into BOUQUET_PARTS_LIST values ( 'HT002', 'BA003', 3 );
insert into BOUQUET_PARTS_LIST values ( 'HT002', 'GP001', 6 );

insert into BOUQUET_PARTS_LIST values ( 'HT003', 'BA003', 1 );

insert into BOUQUET_PARTS_LIST values ( 'HT004', 'BA002', 3 );
insert into BOUQUET_PARTS_LIST values ( 'HT004', 'BA003', 5 );
insert into BOUQUET_PARTS_LIST values ( 'HT004', 'GP001', 3 );
insert into BOUQUET_PARTS_LIST values ( 'HT004', 'CN002', 3 );

insert into BOUQUET_PARTS_LIST values ( 'HT005', 'CN001', 6 );
insert into BOUQUET_PARTS_LIST values ( 'HT005', 'CN002', 6 );

insert into BOUQUET_PARTS_LIST values ( 'HT006', 'BA001', 6 );
insert into BOUQUET_PARTS_LIST values ( 'HT006', 'BA002', 4 );
insert into BOUQUET_PARTS_LIST values ( 'HT006', 'BA003', 3 );
insert into BOUQUET_PARTS_LIST values ( 'HT006', 'GP001', 3 );
insert into BOUQUET_PARTS_LIST values ( 'HT006', 'CN001', 2 );
insert into BOUQUET_PARTS_LIST values ( 'HT006', 'CN002', 5 );

insert into BOUQUET_PARTS_LIST values ( 'HT007', 'BA001', 40 );
insert into BOUQUET_PARTS_LIST values ( 'HT007', 'BA002', 10 );
insert into BOUQUET_PARTS_LIST values ( 'HT007', 'BA003', 10 );

-- 受注履歴(お届け日順)
delete from ORDER_FROM_CUSTOMER;
insert into ORDER_FROM_CUSTOMER values ( '20200301-000001', 20200301, 1, 'HT001',  1, 20200430, 1, "メッセージ00001", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200302-000001', 20200302, 1, 'HT001',  2, 20200430, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200303-000001', 20200303, 1, 'HT001',  3, 20200430, 1, "メッセージ00003", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000001', 20200304, 1, 'HT001',  4, 20200430, 1, "メッセージ00004", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000001', 20200305, 1, 'HT001',  5, 20200430, 1, "メッセージ00005", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200306-000001', 20200306, 1, 'HT004',  6, 20200430, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200307-000001', 20200307, 2, 'HT004',  7, 20200430, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200308-000001', 20200308, 2, 'HT004',  8, 20200430, 1, "メッセージ00008", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200309-000001', 20200309, 2, 'HT005',  9, 20200430, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200310-000001', 20200310, 2, 'HT005', 10, 20200430, 1, "メッセージ00010", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200301-000002', 20200301, 2, 'HT005', 11, 20200430, 1, "メッセージ00011", 0 );

insert into ORDER_FROM_CUSTOMER values ( '20200302-000002', 20200302, 2, 'HT001', 12, 20200501, 1, "メッセージ00012", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200303-000002', 20200303, 2, 'HT001', 13, 20200501, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000002', 20200304, 3, 'HT001', 14, 20200501, 1, "メッセージ00014", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000002', 20200305, 3, 'HT001', 15, 20200501, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200306-000002', 20200306, 3, 'HT001', 16, 20200501, 1, "メッセージ00016", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200307-000002', 20200307, 3, 'HT003', 17, 20200501, 1, "メッセージ00017", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200308-000002', 20200308, 3, 'HT003', 18, 20200501, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200309-000002', 20200309, 3, 'HT003', 19, 20200501, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200310-000002', 20200310, 1, 'HT004',  1, 20200501, 1, "メッセージ00020", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200301-000003', 20200301, 1, 'HT005',  2, 20200501, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200302-000003', 20200302, 1, 'HT005',  3, 20200501, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200303-000003', 20200303, 1, 'HT006',  4, 20200501, 1, "メッセージ00023", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000003', 20200304, 1, 'HT006',  5, 20200501, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000003', 20200305, 1, 'HT006',  6, 20200501, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200306-000003', 20200306, 2, 'HT006',  7, 20200501, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200307-000003', 20200307, 2, 'HT006',  8, 20200501, 1, "メッセージ00027", 0 );

insert into ORDER_FROM_CUSTOMER values ( '20200308-000003', 20200308, 2, 'HT001',  9, 20200502, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200309-000003', 20200309, 2, 'HT001', 10, 20200502, 1, "メッセージ00029", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200310-000003', 20200310, 2, 'HT001', 11, 20200502, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200301-000004', 20200301, 2, 'HT001', 12, 20200502, 1, "メッセージ00031", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200302-000004', 20200302, 2, 'HT002', 13, 20200502, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200303-000004', 20200303, 3, 'HT002', 14, 20200502, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000004', 20200304, 3, 'HT004', 15, 20200502, 1, "メッセージ00034", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000004', 20200305, 3, 'HT004', 16, 20200502, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200306-000004', 20200306, 3, 'HT004', 17, 20200502, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200307-000004', 20200307, 3, 'HT004', 18, 20200502, 1, "メッセージ00037", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200308-000004', 20200308, 3, 'HT006', 19, 20200502, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200309-000004', 20200309, 1, 'HT006',  1, 20200502, 1, "メッセージ00039", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200310-000004', 20200310, 1, 'HT006',  2, 20200502, 1, "メッセージ00040", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200301-000005', 20200301, 1, 'HT007',  3, 20200502, 1, '', 0 );

insert into ORDER_FROM_CUSTOMER values ( '20200302-000005', 20200302, 1, 'HT001',  4, 20200503, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200303-000005', 20200303, 1, 'HT001',  5, 20200503, 1, "メッセージ00043", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000005', 20200304, 1, 'HT002',  6, 20200503, 1, "メッセージ00044", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000005', 20200305, 2, 'HT002',  7, 20200503, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200306-000005', 20200306, 2, 'HT002',  8, 20200503, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200307-000005', 20200307, 2, 'HT002',  9, 20200503, 1, "メッセージ00047", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200308-000005', 20200308, 2, 'HT003', 10, 20200503, 1, "メッセージ00048", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200309-000005', 20200309, 2, 'HT003', 11, 20200503, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200310-000005', 20200310, 2, 'HT004', 12, 20200503, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200301-000006', 20200301, 2, 'HT004', 13, 20200503, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200302-000006', 20200302, 3, 'HT005', 14, 20200503, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200303-000006', 20200303, 3, 'HT005', 15, 20200503, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000006', 20200304, 3, 'HT005', 16, 20200503, 1, "メッセージ00054", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000006', 20200305, 3, 'HT005', 17, 20200503, 1, '', 0 );

insert into ORDER_FROM_CUSTOMER values ( '20200306-000006', 20200306, 3, 'HT003', 18, 20200504, 1, "メッセージ00056", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200307-000006', 20200307, 3, 'HT003', 19, 20200504, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200308-000006', 20200308, 1, 'HT003',  1, 20200504, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200309-000006', 20200309, 1, 'HT007',  2, 20200504, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200310-000006', 20200310, 1, 'HT007',  3, 20200504, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200301-000007', 20200301, 1, 'HT007',  4, 20200504, 1, "メッセージ00061", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200302-000007', 20200302, 1, 'HT007',  5, 20200504, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200303-000007', 20200303, 1, 'HT007',  6, 20200504, 1, "メッセージ00063", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000007', 20200304, 2, 'HT007',  7, 20200504, 1, "メッセージ00064", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000007', 20200305, 2, 'HT007',  8, 20200504, 1, "メッセージ00065", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200306-000007', 20200306, 2, 'HT007',  9, 20200504, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200307-000007', 20200307, 2, 'HT007', 10, 20200504, 1, "メッセージ00067", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200308-000007', 20200308, 2, 'HT007', 11, 20200504, 1, "メッセージ00068", 0 );

insert into ORDER_FROM_CUSTOMER values ( '20200309-000007', 20200309, 2, 'HT001', 12, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200310-000007', 20200310, 2, 'HT001', 13, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200301-000008', 20200301, 3, 'HT001', 14, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200302-000008', 20200302, 3, 'HT001', 15, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200303-000008', 20200303, 3, 'HT001', 16, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000008', 20200304, 3, 'HT001', 17, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000008', 20200305, 3, 'HT006', 18, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200306-000008', 20200306, 3, 'HT006', 19, 20200505, 1, "メッセージ00076", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200307-000008', 20200307, 1, 'HT006',  1, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200308-000008', 20200308, 1, 'HT006',  2, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200309-000008', 20200309, 1, 'HT006',  3, 20200505, 1, "メッセージ00079", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200310-000008', 20200310, 1, 'HT007',  4, 20200505, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200301-000009', 20200301, 1, 'HT007',  5, 20200505, 1, "メッセージ00081", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200302-000009', 20200302, 1, 'HT007',  6, 20200505, 1, '', 0 );

insert into ORDER_FROM_CUSTOMER values ( '20200303-000009', 20200303, 2, 'HT001',  7, 20200506, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000009', 20200304, 2, 'HT001',  8, 20200506, 1, "メッセージ00084", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000009', 20200305, 2, 'HT001',  9, 20200506, 1, "メッセージ00085", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200306-000009', 20200306, 2, 'HT001', 10, 20200506, 1, "メッセージ00086", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200307-000009', 20200307, 2, 'HT004', 11, 20200506, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200308-000009', 20200308, 2, 'HT004', 12, 20200506, 1, "メッセージ00088", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200309-000009', 20200309, 2, 'HT004', 13, 20200506, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200310-000009', 20200310, 3, 'HT004', 14, 20200506, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200301-000010', 20200301, 3, 'HT004', 15, 20200506, 1, "メッセージ00091", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200302-000010', 20200302, 3, 'HT004', 16, 20200506, 1, "メッセージ00092", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200303-000010', 20200303, 3, 'HT006', 17, 20200506, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200304-000010', 20200304, 3, 'HT006', 18, 20200506, 1, '', 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200305-000010', 20200305, 3, 'HT006', 19, 20200506, 1, "メッセージ00095", 0 );
insert into ORDER_FROM_CUSTOMER values ( '20200306-000010', 20200306, 1, 'HT006',  1, 20200506, 1, '', 0 );

-- 発注履歴
delete from ORDERS_TO_SUPPLIER;
insert into ORDERS_TO_SUPPLIER values ( '20200310-000001', 20200425, 1, 20200430, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000002', 20200425, 1, 20200501, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000003', 20200425, 1, 20200502, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000004', 20200425, 1, 20200503, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000005', 20200425, 1, 20200503, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000006', 20200425, 1, 20200503, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000007', 20200425, 1, 20200506, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000008', 20200425, 2, 20200430, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000009', 20200425, 2, 20200501, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000010', 20200425, 2, 20200502, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000011', 20200425, 2, 20200503, 0 );
insert into ORDERS_TO_SUPPLIER values ( '20200310-000012', 20200425, 2, 20200505, 0 );

-- 発注明細
delete from ORDER_DETAILS_TO_SUPPLIER;
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000001', 1, 'BA001', 2, 1 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000001', 2, 'BA002', 1, 2 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000001', 3, 'BA003', 1, 3 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000001', 4, 'GP001', 1, 4 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000002', 1, 'BA001', 3, 5 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000003', 1, 'BA001', 2, 6 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000004', 1, 'BA001', 2, 7 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000005', 1, 'BA002', 1, 8 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000005', 2, 'BA003', 2, 9 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000006', 1, 'BA002', 1, 10 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000007', 1, 'BA001', 1, 11 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000008', 1, 'CN001', 1, 12 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000008', 2, 'CN002', 1, 13 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000009', 1, 'CN002', 1, 14 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000010', 1, 'GP001', 1, 15 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000010', 2, 'CN001', 1, 16 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000011', 1, 'CN002', 1, 17 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000012', 1, 'GP001', 1, 18 );
insert into ORDER_DETAILS_TO_SUPPLIER values ( '20200310-000012', 2, 'CN002', 1, 19 );

-- 在庫アクション
delete from INVENTORY_ACTIONS;
insert into INVENTORY_ACTIONS values ( 20200430, 1, 'BA001', 20200430,  1, 200, 200 );
insert into INVENTORY_ACTIONS values ( 20200430, 3, 'BA001', 20200430,  1,  20, 180 );
insert into INVENTORY_ACTIONS values ( 20200501, 3, 'BA001', 20200430,  1,  50, 130 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'BA001', 20200430,  1,  80,  50 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'BA001', 20200430,  1,  20,  30 );
insert into INVENTORY_ACTIONS values ( 20200503, 6, 'BA001', 20200430,  1,  30,   0 );

insert into INVENTORY_ACTIONS values ( 20200501, 1, 'BA001', 20200501,  5, 300, 300 );
insert into INVENTORY_ACTIONS values ( 20200501, 3, 'BA001', 20200501,  5,   0, 300 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'BA001', 20200501,  5,   0, 300 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'BA001', 20200501,  5,   0, 300 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'BA001', 20200501,  5, 300,   0 );
insert into INVENTORY_ACTIONS values ( 20200504, 6, 'BA001', 20200501,  5,   0,   0 );

insert into INVENTORY_ACTIONS values ( 20200502, 1, 'BA001', 20200502,  6, 200, 200 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'BA001', 20200502,  6,   0, 200 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'BA001', 20200502,  6,   0, 200 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'BA001', 20200502,  6, 100, 100 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'BA001', 20200502,  6, 100,   0 );
insert into INVENTORY_ACTIONS values ( 20200505, 6, 'BA001', 20200502,  6,   0,   0 );

insert into INVENTORY_ACTIONS values ( 20200503, 1, 'BA001', 20200503,  7, 200, 200 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'BA001', 20200503,  7,   0, 200 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'BA001', 20200503,  7,   0, 200 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'BA001', 20200503,  7,  70, 130 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'BA001', 20200503,  7,  40,  90 );
insert into INVENTORY_ACTIONS values ( 20200506, 6, 'BA001', 20200503,  7,  90,   0 );

insert into INVENTORY_ACTIONS values ( 20200506, 1, 'BA001', 20200506, 11, 100, 100 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'BA001', 20200506, 11,   0, 100 );
insert into INVENTORY_ACTIONS values ( 20200507, 3, 'BA001', 20200506, 11,   0, 100 );
insert into INVENTORY_ACTIONS values ( 20200508, 3, 'BA001', 20200506, 11,   0, 100 );
insert into INVENTORY_ACTIONS values ( 20200509, 3, 'BA001', 20200506, 11,   0, 100 );
insert into INVENTORY_ACTIONS values ( 20200509, 6, 'BA001', 20200506, 11, 100,   0 );

insert into INVENTORY_ACTIONS values ( 20200430, 1, 'BA002', 20200430,  2, 100, 100 );
insert into INVENTORY_ACTIONS values ( 20200430, 3, 'BA002', 20200430,  2,   9,  91 );
insert into INVENTORY_ACTIONS values ( 20200501, 3, 'BA002', 20200430,  2,  23,  68 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'BA002', 20200430,  2,  34,  34 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'BA002', 20200430,  2,   6,  28 );
insert into INVENTORY_ACTIONS values ( 20200503, 6, 'BA002', 20200430,  2,  28,   0 );

insert into INVENTORY_ACTIONS values ( 20200503, 1, 'BA002', 20200503,  8, 100, 100 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'BA002', 20200503,  8,   0, 100 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'BA002', 20200503,  8, 100,   0 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'BA002', 20200503,  8,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'BA002', 20200503,  8,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200506, 6, 'BA002', 20200503,  8,   0,   0 );

insert into INVENTORY_ACTIONS values ( 20200503, 1, 'BA002', 20200503, 10, 100, 100 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'BA002', 20200503, 10,   0, 100 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'BA002', 20200503, 10,   0, 100 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'BA002', 20200503, 10,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'BA002', 20200503, 10,  34,  16 );
insert into INVENTORY_ACTIONS values ( 20200506, 6, 'BA002', 20200503, 10,  16,   0 );

insert into INVENTORY_ACTIONS values ( 20200430, 1, 'BA003', 20200430,  3, 100, 100 );
insert into INVENTORY_ACTIONS values ( 20200430, 3, 'BA003', 20200430,  3,  15,  85 );
insert into INVENTORY_ACTIONS values ( 20200501, 3, 'BA003', 20200430,  3,  23,  62 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'BA003', 20200430,  3,  45,  17 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'BA003', 20200430,  3,  17,   0 );
insert into INVENTORY_ACTIONS values ( 20200503, 6, 'BA003', 20200430,  3,   0,   0 );

insert into INVENTORY_ACTIONS values ( 20200503, 1, 'BA003', 20200503,  9, 200, 200 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'BA003', 20200503,  9,   7, 193 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'BA003', 20200503,  9, 103,  90 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'BA003', 20200503,  9,  45,  45 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'BA003', 20200503,  9,  42,   3 );
insert into INVENTORY_ACTIONS values ( 20200506, 6, 'BA003', 20200503,  9,   3,   0 );

insert into INVENTORY_ACTIONS values ( 20200430, 1, 'GP001', 20200430,  4,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200430, 3, 'GP001', 20200430,  4,   9,  41 );
insert into INVENTORY_ACTIONS values ( 20200501, 3, 'GP001', 20200430,  4,  18,  23 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'GP001', 20200430,  4,  23,   0 );
insert into INVENTORY_ACTIONS values ( 20200502, 6, 'GP001', 20200430,  4,   0,   0 );

insert into INVENTORY_ACTIONS values ( 20200502, 1, 'GP001', 20200502, 15,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'GP001', 20200502, 15,  10,  40 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'GP001', 20200502, 15,  30,  10 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'GP001', 20200502, 15,   0,  10 );
insert into INVENTORY_ACTIONS values ( 20200504, 6, 'GP001', 20200502, 15,  10,   0 );

insert into INVENTORY_ACTIONS values ( 20200505, 1, 'GP001', 20200505, 18,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'GP001', 20200505, 18,  15,  35 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'GP001', 20200505, 18,  30,   5 );
insert into INVENTORY_ACTIONS values ( 20200507, 3, 'GP001', 20200505, 18,   0,   5 );
insert into INVENTORY_ACTIONS values ( 20200507, 6, 'GP001', 20200505, 18,   5,   0 );

insert into INVENTORY_ACTIONS values ( 20200430, 1, 'CN001', 20200430, 12,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200430, 3, 'CN001', 20200430, 12,  24,  26 );
insert into INVENTORY_ACTIONS values ( 20200501, 3, 'CN001', 20200430, 12,  22,   4 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'CN001', 20200430, 12,   4,   0 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'CN001', 20200430, 12,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'CN001', 20200430, 12,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'CN001', 20200430, 12,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200505, 6, 'CN001', 20200430, 12,   0,   0 );

insert into INVENTORY_ACTIONS values ( 20200502, 1, 'CN001', 20200502, 16,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'CN001', 20200502, 16,   2,  48 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'CN001', 20200502, 16,  24,  24 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'CN001', 20200502, 16,   0,  24 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'CN001', 20200502, 16,  10,  14 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'CN001', 20200502, 16,   8,   6 );
insert into INVENTORY_ACTIONS values ( 20200507, 3, 'CN001', 20200502, 16,   0,   6 );
insert into INVENTORY_ACTIONS values ( 20200507, 6, 'CN001', 20200502, 16,   6,   0 );

insert into INVENTORY_ACTIONS values ( 20200430, 1, 'CN002', 20200430, 13,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200430, 3, 'CN002', 20200430, 13,  33,  17 );
insert into INVENTORY_ACTIONS values ( 20200501, 3, 'CN002', 20200430, 13,  17,   0 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'CN002', 20200430, 13,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'CN002', 20200430, 13,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'CN002', 20200430, 13,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'CN002', 20200430, 13,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200505, 6, 'CN002', 20200430, 13,   0,   0 );

insert into INVENTORY_ACTIONS values ( 20200501, 1, 'CN002', 20200501, 14,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200501, 3, 'CN002', 20200501, 14,  23,  27 );
insert into INVENTORY_ACTIONS values ( 20200502, 3, 'CN002', 20200501, 14,  27,   0 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'CN002', 20200501, 14,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'CN002', 20200501, 14,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'CN002', 20200501, 14,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'CN002', 20200501, 14,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200506, 6, 'CN002', 20200501, 14,   0,   0 );

insert into INVENTORY_ACTIONS values ( 20200503, 1, 'CN002', 20200503, 17,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200503, 3, 'CN002', 20200503, 17,  30,  20 );
insert into INVENTORY_ACTIONS values ( 20200504, 3, 'CN002', 20200503, 17,   0,  20 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'CN002', 20200503, 17,  20,   0 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'CN002', 20200503, 17,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200507, 3, 'CN002', 20200503, 17,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200508, 3, 'CN002', 20200503, 17,   0,   0 );
insert into INVENTORY_ACTIONS values ( 20200508, 6, 'CN002', 20200503, 17,   0,   0 );

insert into INVENTORY_ACTIONS values ( 20200505, 1, 'CN002', 20200505, 19,  50,  50 );
insert into INVENTORY_ACTIONS values ( 20200505, 3, 'CN002', 20200505, 19,   5,  45 );
insert into INVENTORY_ACTIONS values ( 20200506, 3, 'CN002', 20200505, 19,  38,   7 );
insert into INVENTORY_ACTIONS values ( 20200507, 3, 'CN002', 20200505, 19,   0,   7 );
insert into INVENTORY_ACTIONS values ( 20200508, 3, 'CN002', 20200505, 19,   0,   7 );
insert into INVENTORY_ACTIONS values ( 20200509, 3, 'CN002', 20200505, 19,   0,   7 );
insert into INVENTORY_ACTIONS values ( 20200510, 3, 'CN002', 20200505, 19,   0,   7 );
insert into INVENTORY_ACTIONS values ( 20200510, 6, 'CN002', 20200505, 19,   7,   0 );


commit;
-- vacuum;

-- .save TestData.db
