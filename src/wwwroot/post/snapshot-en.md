Hello Everyone,

This is my very first Dev Community and English article. I hope this tutorial may help you on Elasticsearch snapshots.

If you want to read this article in Turkish please looking my personal website > [Elasticsearh Veri Yedekleme Yöntemleri - Snapshot](http://kolcuoglu.net/Blog/elasticsearch-veri-yedekleme-yontemleri-snapshot-7)

So, let's get starting. :blush:

![](http://kolcuoglu.net/images/blog/elasticsearch-snapshot/snapshot_lifecycle.png)

Snapshot is an official feature of Elasticsearch with Basic Plan (free to use)

We may take a snapshot with Kibana UI, Elasticsearch API, or scripts. In this document, We will use Kibana UI to prepare to take the
snapshot process. But Elasticsearch API has the same parameters and We will share a sample API request.

**IMPORTANT!**
- A snapshot of an index created in 6.x can be restored to 7.x.
- A snapshot of an index created in 5.x can be restored to 6.x.
- A snapshot of an index created in 2.x can be restored to 5.x.
- A snapshot of an index created in 1.x can be restored to 2.x.

On the Kibana side, we may access, manage, and configure snapshots with the 'Management' > 'Snapshot and Restore' section.

![](http://kolcuoglu.net/images/blog/elasticsearch-snapshot/snapshots_section.png)

But first, we must create a repository for creating snapshots. We may create repositories with the 'Repositories' section on the 'Snapshot and Restore' page.
Click the 'Register a repository' button to create a repository. When we click that button, this section will be shown;

![](http://kolcuoglu.net/images/blog/elasticsearch-snapshot/create_repository_section.png)

**Repository Types:**

- **Shared File System:** This option uses the machine's file system and store snapshots on the file system.

- **Read-only URL:** This option wants a URL (http/https/ftp/file) to store snapshots. This URL might be AWS, Azure, Google Cloud like systems, or a
git repository.

> Also Elasticsearch offers us specialized plugins for cloud systems such as AWS, Azure for more effective use in this part.

### Shared File System:

![](http://kolcuoglu.net/images/blog/elasticsearch-snapshot/shared_file_system_option.png)

**Parameters:**
- **File System Location (Required):** This parameter a folder name of the stored data. This will be created inside of elasticsearch directory.

- **Snapshot Compression:** Turns on compression of the snapshot files. Compression is applied only to metadata files (index mapping and settings). Data files are not compressed. Defaults to true.

- **Chunk Size:** Big files can be broken down into chunks during snapshotting if needed. Specify the chunk size as a value and unit, for example, 1GB, 10MB, 5KB, 500B. Defaults to null (unlimited chunk size).

- **Max Snapshot Bytes Per Second:** Throttles per node restore rate. Defaults to 40MB per second.

- **Max Restore Bytes Per Second:** Throttles per node snapshot rate. Defaults to 40MB per second.

- **Read-Only:** Makes repository read-only. Defaults to false.

After the configuration, the repository will be created. But this process will not enough for taking snapshots. We must create a 'Policy' to complete this process.

**Create a Policy:**

Policies a process configuration of how snapshots will create.

![](http://kolcuoglu.net/images/blog/elasticsearch-snapshot/create_policy_part1.png)

**Parameters - Part 1:**

- **Policy Name (Required):** Unique name of the Policy.

- **Snapshot Name (Required):** Name automatically assigned to each snapshot created by the policy. This value supports the same date math supported in index names. To prevent conflicting snapshot names, a UUID is automatically appended to each snapshot name.

- **Repository (Required):** Repository used to store snapshots created by this policy. This repository must exist prior to the policy’s creation.

- **Schedule (Required):** Periodic or absolute schedule at which the policy creates snapshots and deletes expired snapshots. Schedule changes to existing policies are applied immediately.

![](http://kolcuoglu.net/images/blog/elasticsearch-snapshot/create_policy_part2.png)

**Parameters - Part 2:**

- **Indices:** Array of index names or wildcard pattern of index names included in snapshots.

- **Ignore Unavailable Indices:** If true, missing indices do not cause snapshot creation to fail and return an error. Defaults to false.

- **Allow Partial Indices:** Allows snapshots of indices with primary shards that are unavailable. Otherwise, the entire snapshot will fail.

- **Include Global State:** If true, cluster states are included in snapshots. Defaults to false.

![](http://kolcuoglu.net/images/blog/elasticsearch-snapshot/create_policy_part3.png)

**Parameters - Part 3:**

- **Expiration:** Time period after which a snapshot is considered expired and eligible for deletion.

- **Snapshots to Retain:**

    - **Minimum Count:** Minimum number of snapshots to retain, even if the snapshots have expired.

    - **Maximum Count:** Maximum number of snapshots to retain, even if the snapshots have not yet expired. If the number of snapshots in the repository exceeds this limit, the policy retains the most recent snapshots and deletes older snapshots.

After the configurations, Kibana shows us the API request version of these configurations. For this document, the request in code is like below;

``` json
PUT _slm/policy/daily-snapshot-backup-snapshot-training
{
  "name": "dsbst-{now}",
  "schedule": "0 30 4 * * ?",
  "repository": "backup-snapshot-training",
  "config": {
    "ignore_unavailable": true
  },
  "retention": {
    "expire_after": "30d",
    "min_count": 1,
    "max_count": 10
  }
}
```

And finally, the policy created. On the configured time, elasticsearch will be triggered to take a snapshot automatically.

![](http://kolcuoglu.net/images/blog/elasticsearch-snapshot/policy_view.png)

We may also start taking the snapshot policy manually anytime we want.

---

In this tutorial, I aimed to give you simple information about Snapshot, which is the most used method to backup on Elasticsearch and is recommended by Elasticsearch itself. For your questions, you can contact me via social media and e-mail.

Keep Coding :blush:

## References

- [Elasticsearch Docs - Snapshot Register Repository](https://www.elastic.co/guide/en/elasticsearch/reference/7.6/snapshots-register-repository.html)

- [Elasticsearch Docs - SLM API PUT Policy](https://www.elastic.co/guide/en/elasticsearch/reference/7.6/slm-api-put-policy.html)

- [Elasticsearch Docs - Repository](https://www.elastic.co/guide/en/elasticsearch/plugins/7.6/repository.html)

- [Elasticsearch Docs - Snapshots, Take Snapshot](https://www.elastic.co/guide/en/elasticsearch/reference/current/snapshots-take-snapshot.html)