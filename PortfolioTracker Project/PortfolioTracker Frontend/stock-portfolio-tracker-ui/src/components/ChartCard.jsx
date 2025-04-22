import React from "react";
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer
} from "recharts";
import './ChartCard.css';

const ChartCard = ({ ticker, chartData }) => {
  if (!Array.isArray(chartData) || chartData.length === 0) {
    return (
      <div className="chart-card">
        <h4>{ticker}</h4>
        <p>No chart data available.</p>
      </div>
    );
  }

  const formattedData = chartData.map(item => ({
    time: new Date(item.date).toLocaleTimeString([], {
      hour: "2-digit",
      minute: "2-digit"
    }),
    price: item.closingPrice
  }));

  return (
    <div className="chart-card">
      <h4>{ticker}</h4>
      <ResponsiveContainer width="100%" height={230}>
        <LineChart data={formattedData}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="time" />
          <YAxis domain={["auto", "auto"]} />
          <Tooltip formatter={(value) => [`₹${value.toFixed(2)}`, "Price"]} />
          <Line
            type="monotone"
            dataKey="price"
            stroke="#3b82f6"
            strokeWidth={2}
            dot={false}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
};

export default ChartCard;