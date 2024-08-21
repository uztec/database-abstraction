
DROP TABLE IF EXISTS user_test CASCADE;

CREATE TABLE user_test (
    id_user SERIAL4,
    cod_user INT NOT NULL,
    user_name VARCHAR (64) NOT NULL,
    cod_user_ref BIGINT NULL,
    pasword_md5 CHAR(32) NOT NULL,
    input_date TIMESTAMP NOT NULL,
    user_status CHAR(1) NOT NULL,
    PRIMARY KEY(id_user)
);

INSERT INTO user_test (cod_user, user_name, cod_user_ref, pasword_md5, input_date, user_status)
    VALUES(44, 'First User Used For Tests', 1234567890123456, MD5('password123'), '2020-03-29 18:03:12', 'A')
