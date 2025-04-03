import React, { useState, useEffect } from "react";
import webSocketService from "../services/WebSocketService";
import "./MemberList.css"; // Import the CSS file

const MemberList = () => {
    const [members, setMembers] = useState([]);

    useEffect(() => {
        webSocketService.connect();

        const updateMembers = (data) => {
            if (data?.members && Array.isArray(data.members)) {
                setMembers(data.members);
            } else {
                console.error("Invalid WebSocket data format:", data);
                setMembers([]);
            }
        };

        webSocketService.addListener(updateMembers);

        return () => {
            webSocketService.removeListener(updateMembers);
        };
    }, []);

    return (
        <div className="member-list-container">
            <h1>Live Member List</h1>

            {members.length === 0 ? (
                <p>No members available.</p>
            ) : (
                <table className="member-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {members.map((member, index) => (
                            <tr key={member.Id}>
                                <td>{member.Id}</td>
                                <td>{member.Name}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default MemberList;
