import React, { useEffect, useState } from "react";
import { useAuth } from "../AuthContext";
import {
  subscribeToWebSocket,
  unsubscribeFromWebSocket,
} from "../services/WebSocketService";
import {
  getPortfolioSummary,
  getInitialChartData,
  getHistoricalChartData,
  getAvailableDates,
} from "../services/statistics";
import "./Statistics.css";
import ChartCard from "./ChartCard";
import PortfolioSummaryCard from "./PortfolioSummaryCard";

const Statistics = () => {
  const { token } = useAuth();
  const [summary, setSummary] = useState(null);
  const [chartData, setChartData] = useState({});
  const [filteredTickers, setFilteredTickers] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [availableDates, setAvailableDates] = useState([]);
  const [selectedDate, setSelectedDate] = useState("");
  const [loading, setLoading] = useState(true);

  const fetchStatistics = async () => {
    try {
      const summaryData = await getPortfolioSummary(token);
      const dates = await getAvailableDates(token);
      const defaultDate = dates?.[0] || "";

      const chartResponse = await getInitialChartData(defaultDate, token);

      setSummary(summaryData);
      setAvailableDates(dates);
      setSelectedDate(defaultDate);
      setChartData(chartResponse);
      setFilteredTickers(Object.keys(chartResponse));
    } catch (err) {
      console.error("Error fetching statistics:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStatistics();
    subscribeToWebSocket(fetchStatistics);
    return () => unsubscribeFromWebSocket(fetchStatistics);
  }, [token]);

  const handleSearchChange = (e) => {
    const value = e.target.value;
    setSearchTerm(value);

    const matches = Object.keys(chartData).filter((ticker) =>
      ticker.toLowerCase().includes(value.toLowerCase())
    );
    setFilteredTickers(matches);
  };

  const handleSearchClick = async () => {
    const trimmed = searchTerm.trim().toUpperCase();
    if (!trimmed) return;

    if (chartData[trimmed]) {
      setFilteredTickers([trimmed]);
      return;
    }

    try {
      const newChart = await getHistoricalChartData(trimmed, token, selectedDate);
      if (newChart) {
        const updated = { ...chartData, [trimmed]: newChart };
        setChartData(updated);
        setFilteredTickers([trimmed]);
      } else {
        alert("No chart data found for this ticker.");
      }
    } catch (err) {
      console.error("Error fetching chart data:", err);
      alert("Error fetching historical data.");
    }
  };

  const handleClear = () => {
    setSearchTerm("");
    setFilteredTickers(Object.keys(chartData));
  };

  const handleDateChange = async (e) => {
    const newDate = e.target.value;
    setSelectedDate(newDate);

    try {
      const newChartData = await getInitialChartData( newDate,token);
      setChartData(newChartData);
      setFilteredTickers(Object.keys(newChartData));
    } catch (error) {
      console.error("Failed to fetch chart data for date:", newDate, error);
    }
  };

  if (loading) return <p className="loading-text">Loading statistics...</p>;

  return (
    <div className="statistics-container statistics">
      <h2>Portfolio Summary</h2>
      {summary ? (
        <PortfolioSummaryCard summary={summary} />
      ) : (
        <p>No summary available.</p>
      )}

      <h2>Stock Charts</h2>

     

      <div className="search-section">
        <div className="search-controls">
            <input
                type="text"
                className="search-input"
                placeholder="Search stock ticker..."
                value={searchTerm}
                onChange={handleSearchChange}
            />
            <button className="search-button" onClick={handleSearchClick}>
                Search
            </button>
            <button className="clear-button" onClick={handleClear}>
                Clear
            </button>
            </div>

        <div className="date-select">
            <label htmlFor="date-dropdown">Select Date: </label>
            <select id="date-dropdown" value={selectedDate} onChange={handleDateChange}>
                {availableDates.map((date) => (
                <option key={date} value={date}>
                    {date}
                </option>
                ))}
            </select>
        </div>
      </div>

      <div className="chart-section">
        {filteredTickers.length > 0 ? (
          filteredTickers.map((ticker) => (
            <ChartCard key={ticker} ticker={ticker} chartData={chartData[ticker]} />
          ))
        ) : (
          <p>No matching charts found.</p>
        )}
      </div>
    </div>
  );
};

export default Statistics;
