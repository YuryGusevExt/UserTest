import React, { useState } from 'react';
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    LineElement,
    Title,
    Tooltip,
    Legend,
} from 'chart.js';
import { Bar } from 'react-chartjs-2';

export default function Calc(showFunc, ret, labels, histY, show) {

    const close = () => {
        showFunc(false);
    }

    if (!labels.length || !histY.length || !show)
        return (<p>No data<br /><button onClick={close}>Close</button><br /></p>);

    ChartJS.register(
        CategoryScale,
        LinearScale,
        BarElement,
        LineElement,
        Title,
        Tooltip,
        Legend
    );

    const options = {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            yAxes: {
                title: {
                    display: true,
                    text: 'Users',
                    font: {
                        size: 15
                    }
                },
                ticks: {
                    precision: 0
                }
            },
            xAxes: {
                type: 'linear',
                title: {
                    display: true,
                    text: 'Days',
                    font: {
                        size: 15
                    }
                },
                    ticks: {
                    stepSize: 1,
                    fixedStepSize: 1,
                }
            }
        },
        plugins: {
            legend: {
                display: false,
            },
            title: {
                display: true,
                text: 'Days of life',
            },
        },
    };

    const data = {
        labels,
        datasets: [
            {
                data: histY,
                backgroundColor: '#4A9DFF',
                categoryPercentage: 1.0,
                barPercentage: 1.0
            },
        ],
    };

    return (
        <>
            <button onClick={close}>Close</button>
            <div className="info">
                <br />
                <b>Rolling Retention 7 day:</b> {ret[0]}%<br />
                <b>Median:</b> {ret[1]}<br />
                <b>Percentile, 10%:</b> {ret[2]}<br />
                <b>Percentile, 90%:</b> {ret[3]}</div>
            <div className="chart"><Bar options={options} data={data} /></div>
        </>
    );
}