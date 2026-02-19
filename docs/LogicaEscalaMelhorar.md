Ao selecionar o horário/turno, deve-se fazer um cálculo para os dias que a pessoa 
está de folga, o botao do dia não ficar disponivel. Exemplo:
Ao selecionar o dia 16 com o horario das 07 as 19, deve-se entender que a pessoa entra 
para trabalhar dia 16 as 07 da manhã e sai as 19. Se a escala for 12x36, começaria a contar a partir das 19h
as 36 horas de descanso. O botão do dia 17 ficaria indisponivel e aparecia o botão do dia 18 (a partir das 07 da manha)
Se a pessoa tentar adicionar dia 18 antes da 07 da manhã, deve aparecer um aviso que ainda não deu as horas (escala 12x36)
porém o botão continua disponivel para o dia pq ele pode entrar as 07 da manha. 

A lógica deve ser, se no dia ele não trabalhar as 24 horas cheias, o botão não deve ficar disponivel, se o final da folga
cair na metade de um dia por exemplo (as 15:00), o botão fica disponivel.