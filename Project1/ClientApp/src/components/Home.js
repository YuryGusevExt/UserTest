import React, { useState } from 'react';
import { DataGrid } from '@mui/x-data-grid';
import Calc from '../components/Calc'

let loading = true;

export default function Home() {
    const [datas, setDatas] = useState([])
    const [selectedRow, setSelectedRow] = useState(0)
    const [error, setError] = useState(undefined)
    const [result, setResult] = useState(undefined)
    const [showCalc, setShowCalc] = useState(false);
    const [changed, setChanged] = useState(false);
    const [ret, setRet] = useState([0, 0, 0, 0]);
    const [histX, setHistX] = useState([]);
    const [histY, setHistY] = useState([]);

    if (loading) {
        loading = false;
        populateData();
    }

    async function populateData() {
        fetch('userdata').then(function (response) {
            response.json().then(function (text) {
                if (text.error != undefined)
                    setError(text.error);
                else {
                    let tmp = text.datas.map((x) => ({ userID: x.userID, registerDate: new Date(x.registerDate), lastSeenDate: new Date(x.lastSeenDate) }));
                    setDatas(tmp);
                }
            });
        });
    }

    async function populateCalc() {
        fetch('userdata/calc').then(function (response) {
            response.json().then(function (text) {
                if (text.error != undefined)
                    setError(text.error);
                else {
                    setRet([text.retention, text.median, text.percentile10, text.percentile90]);
                    setHistX(text.histX);
                    setHistY(text.histY);
                }
            });
        });
    }

    const addRow = () => {
        let id = 1;
        datas.forEach((x) => id = x.userID >= id ? x.userID + 1 : id);
        var today = new Date(Date.now());
        today.setHours(0, 0, 0, 0);
        let tmp = [...datas, { userID: id, registerDate: today, lastSeenDate: today }];
        setDatas(tmp);
        setSelectedRow(id);
        setChanged(true);
        // В data-grid-pro можно сделать с помощью родного api
        setTimeout(() => { document.getElementsByClassName('MuiDataGrid-virtualScroller')[0].scroll(0, tmp.length * 48) }, 333)
    }

    const delRow = () => {
        if (selectedRow && selectedRow[0]) {
            let id = datas.findIndex(x => x.userID == selectedRow[0])
            if (id >= 0) {
                let tmp = [...datas];
                tmp.splice(id, 1);
                setDatas(tmp);
                setSelectedRow(undefined);
                setChanged(true);
            }
        }
    }

    const save = () => {
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(datas)
        };
        fetch('userdata', requestOptions)
            .then(response => {
                if (!response.ok)
                    return response.json().then(text => { throw new Error(Object.values(text.errors).join("; ")) });
                return response.json()
            })
            .then(data => {
                if (data.error) {
                    setResult(undefined);
                    setError(data.error)
                } else {
                    setError(undefined);
                    setResult("Saving: Ok");
                    setChanged(false);
                }
            }).catch((e) => {
                setResult(undefined);
                setError("Error: " + e.message);
            });
    }

    const calc = () => {
        setResult(undefined);
        setError(undefined);
        populateCalc();
        if (error == undefined) setShowCalc(true);
    }

    const processRowUpdate = async (newRow) => {
        let fin = datas.find((x) => x.userID == newRow.userID);
        fin.registerDate = newRow.registerDate;
        fin.lastSeenDate = newRow.lastSeenDate;
        setChanged(true);
        return { ...newRow, isNew: false };
    };

    const selChanged = (id) => {
        setSelectedRow(id);
    };

    const renderDatasTable = () => {
        return (
            <div style={{ height: 400, width: '100%' }}>
                <DataGrid
                    experimentalFeatures={{ newEditingApi: true }}
                    getRowId={(row) => row.userID}
                    rows={datas}
                    columns={columns}
                    onSelectionModelChange={selChanged}
                    disableColumnMenu={true}
                    processRowUpdate={processRowUpdate}
                />
                <button onClick={addRow}>Add row</button>
                <button onClick={delRow} className={"button"}>Del row</button>
                <button onClick={save} className={"button"}>Save</button>
                <button onClick={calc} className={"button"} disabled={changed}>Calculate</button>
            </div>
        );
        //selectionModel={selectedRow}
    }

    const getRet = () => ret;
    const getHX = () => histX;
    const getHY = () => histY;

    const close = () => {
        setError(undefined);
    }

    return (
        <div>
            <div className="header">User data</div>
            <div className="messages">
                {error && <p className="error">{error}<br /><button onClick={close}>Close</button><br /></p>}
                {result && result}
            </div>
            <div className="pages">
                {showCalc ? null : renderDatasTable()}
                {showCalc && Calc(setShowCalc, ret, histX, histY, showCalc)}
            </div>
        </div>
    );
}

const columns = [
    { field: 'userID', headerName: 'UserID', width: 150, sortable: false, hideable: false },
    { field: 'registerDate', headerName: 'Date Registration', width: 150, editable: true, type: 'date', sortable: false, hideable: false },
    { field: 'lastSeenDate', headerName: 'Date Last Activity', width: 150, editable: true, type: "date", sortable: false, hideable: false },
];