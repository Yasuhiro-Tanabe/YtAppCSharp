﻿-- テーブル：CUSTOMERS (得意先)
DROP TABLE IF EXISTS CUSTOMERS;
CREATE TABLE CUSTOMERS (
    ID INT NOT NULL PRIMARY KEY     /* 得意先ID */,
    E_MAIL TEXT NOT NULL     /* メールアドレス */,
    NAME TEXT NOT NULL     /* 氏名 */,
    PASSWORD TEXT     /* パスワード */,
    CARD_NO TEXT NOT NULL     /* クレジットカード番号 */,
    STATUS INT NOT NULL     /* 状態 */
);


-- テーブル：BOUQUET_SET (商品(花束セット))
DROP TABLE IF EXISTS BOUQUET_SET;
CREATE TABLE BOUQUET_SET (
    CODE TEXT NOT NULL PRIMARY KEY     /* 花束コード */,
    NAME TEXT NOT NULL     /* 名称 */,
    IMAGE TEXT     /* イメージ画像 */,
    LEAD_TIME INT NOT NULL     /* 発注リードタイム */,
    STATUS INT NOT NULL     /* 状態 */
);


-- テーブル：BOUQUET_PARTS (単品(花卉そのほか))
DROP TABLE IF EXISTS BOUQUET_PARTS;
CREATE TABLE BOUQUET_PARTS (
    CODE TEXT NOT NULL PRIMARY KEY     /* 花コード */,
    NAME TEXT NOT NULL     /* 花名称 */,
    LEAD_TIME INT NOT NULL     /* 発注リードタイム */,
    NUM_PAR_LOT INT NOT NULL     /* 購入単位数 */,
    EXPIRY_DATE INT NOT NULL     /* 品質維持可能日数 */,
    STATUS INT NOT NULL     /* 状態 */
);


-- テーブル：SHIPPING_ADDRESS (お届け先)
DROP TABLE IF EXISTS SHIPPING_ADDRESS;
CREATE TABLE SHIPPING_ADDRESS (
    ID INT NOT NULL PRIMARY KEY     /* お届け先ID */,
    CUSTOMER_ID INT NOT NULL REFERENCES CUSTOMER(ID)     /* 得意先ID */,
    ADDRESS_1 TEXT NOT NULL     /* お届け先住所1 */,
    ADDRESS_2 TEXT     /* お届け先住所2 */,
    NAME TEXT NOT NULL     /* お届け先氏名 */,
    LATEST_ORDER INT     /* 最新注文日 */
);


-- テーブル：SUPPLIERS (仕入れ先)
DROP TABLE IF EXISTS SUPPLIERS;
CREATE TABLE SUPPLIERS (
    CODE INT NOT NULL PRIMARY KEY     /* 仕入れ先コード */,
    NAME TEXT NOT NULL     /* 仕入れ先名称 */,
    ADDRESS_1 TEXT NOT NULL     /* 仕入れ先住所1 */,
    ADDRESS_2 TEXT     /* 仕入れ先住所2 */,
    TEL TEXT     /* 仕入れ先電話番号 */,
    FAX TEXT     /* 仕入れ先FAX番号 */,
    E_MAIL TEXT     /* メールアドレス */
);


-- テーブル：BOUQUET_PARTS_LIST (商品構成)
DROP TABLE IF EXISTS BOUQUET_PARTS_LIST;
CREATE TABLE BOUQUET_PARTS_LIST (
    BOUQUET_CODE TEXT NOT NULL REFERENCES BOUQUET_SET(CODE)     /* 花束コード */,
    PARTS_CODE TEXT NOT NULL REFERENCES BOUQUET_PARTS(CODE)     /* 花コード */,
    QUANTITY INT NOT NULL     /* 数量 */,
  PRIMARY KEY ( BOUQUET_CODE, PARTS_CODE )
);


-- テーブル：BOUQUET_SUPPLIERS (単品仕入れ先)
DROP TABLE IF EXISTS BOUQUET_SUPPLIERS;
CREATE TABLE BOUQUET_SUPPLIERS (
    SUPPLIER_CODE INT NOT NULL REFERENCES SUPPLIERS(CODE)     /* 仕入れ先コード */,
    BOUQUET_PARTS_CODE TEXT NOT NULL REFERENCES BOUQUET_PARTS(CODE)     /* 花コード */,
  PRIMARY KEY ( SUPPLIER_CODE, BOUQUET_PARTS_CODE )
);


-- テーブル：ORDER_FROM_CUSTOMER (受注履歴)
DROP TABLE IF EXISTS ORDER_FROM_CUSTOMER;
CREATE TABLE ORDER_FROM_CUSTOMER (
    ID TEXT NOT NULL PRIMARY KEY     /* 受注番号 */,
    DATE INT NOT NULL     /* 受付日 */,
    CUSTOMER TEXT NOT NULL REFERENCES CUSTOMER(ID)     /* 得意先ID */,
    BOUQUET TEXT NOT NULL REFERENCES BOUQUET_SET(CODE)     /* 花束コード */,
    SHIPPING_ADDRESS TEXT NOT NULL     /* お届け先ID */,
    SHIPPING_DATE INT NOT NULL     /* お届け日 */,
    HAS_MESSAGE INT NOT NULL     /* メッセージ要否 */,
    MESSAGE TEXT     /* お届けメッセージ */,
    STATUS INT NOT NULL     /* 状態 */
);


-- テーブル：ORDER_DETAILS_FROM_CUSTOMER (受注明細)
DROP TABLE IF EXISTS ORDER_DETAILS_FROM_CUSTOMER;
CREATE TABLE ORDER_DETAILS_FROM_CUSTOMER (
    ORDER_FROM_CUSTOMER TEXT NOT NULL REFERENCES ORDER_FROM_CUSTOMER(ID)     /* 受注番号 */,
    ORDER_INDEX INT NOT NULL     /* 明細番号 */,
    BOUQUET_PARTS_CODE TEXT NOT NULL REFERENCES BOUQUET_PARTS(CODE)     /* 花コード */,
    QUANTITY INT NOT NULL     /* 数量 */,
  PRIMARY KEY ( ORDER_FROM_CUSTOMER, ORDER_INDEX, BOUQUET_PARTS_CODE )
);


-- テーブル：ORDERS_TO_SUPPLIER (発注履歴)
DROP TABLE IF EXISTS ORDERS_TO_SUPPLIER;
CREATE TABLE ORDERS_TO_SUPPLIER (
    ID TEXT NOT NULL PRIMARY KEY     /* 発注番号 */,
    ORDER_DATE INT NOT NULL     /* 発注日 */,
    SUPPLIER TEXT NOT NULL REFERENCES SUPPLIERS(CODE)     /* 仕入れ先ID */,
    DELIVERY_DATE INT NOT NULL     /* 納品希望日 */,
    STATUS INT NOT NULL     /* 状態 */
);


-- テーブル：ORDER_DETAILS_TO_SUPPLIER (発注明細)
DROP TABLE IF EXISTS ORDER_DETAILS_TO_SUPPLIER;
CREATE TABLE ORDER_DETAILS_TO_SUPPLIER (
    ORDER_TO_SUPPLIER_ID TEXT NOT NULL REFERENCES ORDER_TO_SUPPLIER(ID)     /* 発注番号 */,
    ORDER_INDEX INT NOT NULL     /* 明細番号 */,
    PARTS_CODE TEXT NOT NULL REFERENCES BOUQUET_PARTS(CODE)     /* 花コード */,
    LOT_COUNT INT NOT NULL     /* ロット数 */,
    STOCK_LOT_NO INT     /* 在庫ロット番号 */,
  PRIMARY KEY ( ORDER_TO_SUPPLIER_ID, ORDER_INDEX )
);


-- テーブル：STOCK_ACTIONS (在庫アクション履歴)
DROP TABLE IF EXISTS STOCK_ACTIONS;
CREATE TABLE STOCK_ACTIONS (
    ACTION_DATE INT NOT NULL     /* 基準日 */,
    ACTION INT NOT NULL     /* アクション */,
    BOUQUET_PARTS_CODE TEXT NOT NULL REFERENCES BOUQUET_PARTS(CODE)     /* 花コード */,
    ARRIVAL_DATE INT NOT NULL     /* 入荷日 */,
    LOT_NO INT NOT NULL     /* 在庫ロット番号 */,
    QUANTITY INT NOT NULL     /* 数量 */,
    REMAIN INT NOT NULL     /* 残数 */,
  PRIMARY KEY ( ACTION_DATE, ACTION, BOUQUET_PARTS_CODE, ARRIVAL_DATE, LOT_NO )
);


-- テーブル：SESSIONS (セッション)
DROP TABLE IF EXISTS SESSIONS;
CREATE TABLE SESSIONS (
    ID INT NOT NULL PRIMARY KEY     /* セッションID */,
    CUSTOMER INT NOT NULL REFERENCES COSTMERS(ID)     /* 得意先ID */,
    LOGGED_IN TEXT NOT NULL     /* ログイン日時 */,
    LATEST_ACCESS_TIME TEXT NOT NULL     /* 最終アクセス日時 */
);


-- テーブル：DATE_MASTER (日付マスタ)
DROP TABLE IF EXISTS DATE_MASTER;
CREATE TABLE DATE_MASTER (
    DATE INT NOT NULL     /* 日付 */,
    DATE_INDEX INT NOT NULL     /* 連番 */,
  PRIMARY KEY ( DATE, DATE_INDEX )
);


-- テーブル：SEQUENCES (連番管理)
DROP TABLE IF EXISTS SEQUENCES;
CREATE TABLE SEQUENCES (
    NAME TEXT NOT NULL PRIMARY KEY     /* シーケンス名 */,
    VALUE INT     /* 現在値 */
);


-- テーブル：STRINGS (文字列リソース)
DROP TABLE IF EXISTS STRINGS;
CREATE TABLE STRINGS (
    NAME TEXT NOT NULL PRIMARY KEY     /* リソース名 */,
    VALUE TEXT NOT NULL     /* リソース文字列 */
);


-- テーブル：PARAMETERS (パラメータ)
DROP TABLE IF EXISTS PARAMETERS;
CREATE TABLE PARAMETERS (
    NAME TEXT NOT NULL PRIMARY KEY     /* パラメータ名 */,
    TYPE INT NOT NULL     /* データ型 */,
    VALUE_NUMBER INT     /* パラメータ値(数値) */,
    VALUE_STRING TEXT     /* パラメータ値(文字列) */
);


