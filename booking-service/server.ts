import express from 'express';

const app = express();
const PORT = process.env.PORT || 5002;

app.use(express.json());

app.get('/api/health', (_req, res) => {
  res.json({ status: 'ok', service: 'booking-service' });
});

app.listen(PORT, () => {
  // eslint-disable-next-line no-console
  console.log(`Booking Service listening on port ${PORT}`);
});

export default app;
