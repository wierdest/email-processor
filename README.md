# 📫 EmailProcessor

Um **worker .NET** que lê e processa e-mails com anexos via **IMAP** e envia os dados para uma camada de aplicação usando os princípios de **Clean Architecture** e **Vertical Slice Architecture**.

---

## 📌 Objetivo

O objetivo deste projeto é:

- Ler e-mails de uma caixa de entrada via **IMAP**
- Extrair anexos relevantes
- Processar os dados via um **Command Handler**
- **Excluir os e-mails** da caixa após o processamento
- Executar esse processo periodicamente via **cron job** em um container Podman

---

## 🧱 Arquitetura

- **Domain**: contém as entidades e regras de negócio (ex: `EmailMessage`)
- **Application**: contém os command handlers (ex: `ProcessEmailCommand`)
- **Infrastructure**: contém a implementação concreta de leitura via IMAP (usando MailKit)
- **Worker**: serviço `BackgroundService` que orquestra a leitura e o processamento

---

## 🐳 Docker / Podman

### 🔨 Build da imagem:

```bash
podman build -t email-processor .
```

### 🚀 Rodar o container (dentro de um pod) usando `.env`:

```bash
podman run --rm   --pod mypod   --env-file .env   email-processor
```

> As variáveis de ambiente seguem o padrão de configuração do .NET para settings fortemente tipados.

---

## ⏱️ Cron Job

Esse worker foi projetado para ser executado periodicamente via **cron job no host**.

A configuração recomendada já inclui os seguintes scripts:

- `start-email-processor.sh`: constrói a imagem, registra o cron job e prepara o ambiente.
- `stop-email-processor.sh`: remove o cron job e limpa o ambiente.
- `run-email-processor.sh`: executa o container com base na imagem e no `.env`.

Para evitar que múltiplas execuções concorrentes causem conflitos ou inconsistências, o script `run-email-processor.sh` utiliza um **arquivo de lock** (`/tmp/email-processor.lock`). Esse lock impede que a tarefa seja executada caso outra instância ainda esteja rodando.

Além disso, o script detecta **locks órfãos** (processos que não existem mais) e os remove automaticamente após um tempo configurável (ex: 15 minutos), garantindo que execuções futuras não fiquem bloqueadas indevidamente.

## 📦 Armazenamento futuro

Planejamos incluir o uso de um **banco de dados não relacional** (como **MongoDB**) para persistir as mensagens de e-mail processadas.

> ✅ **MongoDB tem suporte a TTL (Time To Live)** usando índices com data de expiração automática.

Referência: [MongoDB TTL Indexes](https://www.mongodb.com/docs/manual/core/index-ttl/)

---

## ✅ Status

- [x] Leitura de e-mails via IMAP (MailKit)
- [x] Processamento de anexos
- [x] Exclusão dos e-mails após leitura
- [x] Injeção de dependência com configurações fortemente tipadas
- [x] Container funcional via Podman
- [ ] Integração com banco de dados (futuro)