FROM node:24-alpine

WORKDIR /app/poetry-global-frontend

# install deps (cached)
COPY ./poetry-global-frontend/package.json poetry-global-frontend/package-lock.json* ./
RUN npm install

# copy rest of the app
COPY poetry-global-frontend/ .

# Vite dev server port
EXPOSE 5173

# run dev server, accessible outside container
CMD ["npm", "run", "dev", "--", "--host"]